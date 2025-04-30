using System.Security.Claims;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Constants;
using WebApplication1.Data;

namespace WebApplication1.Controllers;

public class ScriptStageController(ApplicationDbContext context) : Controller
{
    private const string InvalidArgumentsMessage = "Wrong arguments!";

    [HttpPost]
    [Authorize(Roles = AuthConstants.ManagerRole)]
    public async Task<IActionResult> CreateVideo(Guid projectId, string script, string reference)
    {
        if (!ModelState.IsValid)
            return BadRequest(InvalidArgumentsMessage);

        var project = await context.Projects.Include(p => p.Videos).FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
            return BadRequest("Invalid project state!");

        var maxVideoNumber = project.Videos.Count != 0
            ? project.Videos.Max(v => v.VideoNumber)
            : 0;

        project.Videos.Add(new Video
        {
            VideoNumber = maxVideoNumber + 1,
            Script = script,
            Reference = new Uri(reference),
            Status = ScriptStatus.Correction
        });

        await context.SaveChangesAsync();

        return RedirectToAction("Details", "Project", new { id = projectId });
    }

    [HttpPost]
    [Authorize(Roles = AuthConstants.ManagerRole)]
    public async Task<IActionResult> UpdateVideo(Guid projectId, int videoId, string script, string reference)
    {
        if (!ModelState.IsValid)
            return BadRequest(InvalidArgumentsMessage);

        var project = await context.Projects
            .Include(p => p.Videos)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
            return BadRequest("Invalid project!");

        var video = project.Videos.FirstOrDefault(v => v.VideoNumber == videoId);

        if (video == null)
            return NotFound("Invalid video number!");

        video.Script = script;
        video.Reference = new Uri(reference);
        video.Status = ScriptStatus.Correction;

        await context.SaveChangesAsync();

        return RedirectToAction("Details", "Project", new { id = projectId });
    }
    
    [Authorize(Roles = AuthConstants.ClientRole)]
    public async Task<IActionResult> ApproverVideo(Guid projectId, int videoId, ScriptStatus status,
        string clientComment)
    {
        if (!ModelState.IsValid)
            return BadRequest(InvalidArgumentsMessage);

        var project = await context.Projects
            .Include(p => p.Videos)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
            return BadRequest("Invalid project!");

        var video = project.Videos.FirstOrDefault(v => v.VideoNumber == videoId);

        if (video == null)
            return NotFound("Invalid video number!");

        video.Status = status;

        project.ProjectComments.Add(new ProjectComment
        {
            UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            CommentText = clientComment ?? string.Empty,
            CommentStage = ProjectStage.Script,
            RelatedVideoNumber = video.VideoNumber
        });

        if (project.Videos.All(v => v.Status == ScriptStatus.Approved))
            project.ProjectStage = ProjectStage.Filming;

        await context.SaveChangesAsync();

        return RedirectToAction("Details", "Project", new { id = projectId });
    }

    [HttpPost]
    public async Task<IActionResult> PostFilmingComment(Guid projectId, string message)
    {
        if (!ModelState.IsValid)
            return BadRequest(InvalidArgumentsMessage);

        if (string.IsNullOrWhiteSpace(message))
            return RedirectToAction("Details", "Project", new { id = projectId });

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var project = await context.Projects
            .Include(p => p.ProjectComments)
            .Include(p => p.Client)
            .Include(p => p.Manager)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
            return NotFound();

        var comment = new ProjectComment
        {
            ProjectId = project.Id,
            UserId = userId,
            CommentText = message,
            CommentStage = ProjectStage.Filming
        };

        project.ProjectComments.Add(comment);
        await context.SaveChangesAsync();

        return RedirectToAction("Details", "Project", new { id = projectId });
    }

    [HttpPost]
    [Authorize(Roles = AuthConstants.ManagerRole)]
    public async Task<IActionResult> CompleteFilming(Guid projectId)
    {
        if (!ModelState.IsValid)
            return BadRequest(InvalidArgumentsMessage);

        var project = await context.Projects
            .Include(pr => pr.Videos)
            .FirstAsync(pr => pr.Id == projectId);

        if (project is not { ProjectStage: ProjectStage.Filming })
            return NotFound();

        project.ProjectStage = ProjectStage.VideoEditing;

        foreach (var video in project.Videos)
        {
            video.Status = ScriptStatus.Correction;
        }

        await context.SaveChangesAsync();

        return RedirectToAction("Details", "Project", new { id = projectId });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateFinalVideoLink(Guid projectId, int videoId, string finalVideoLink)
    {
        if (!ModelState.IsValid)
            return BadRequest(InvalidArgumentsMessage);

        var project = await context.Projects
            .Include(p => p.Videos)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project?.Videos == null)
            return NotFound();

        var video = project.Videos.FirstOrDefault(v => v.VideoNumber == videoId);

        if (video == null)
            return NotFound("Video is not found!");

        video.VideoUri = new Uri(finalVideoLink);
        await context.SaveChangesAsync();

        return RedirectToAction("Details", "Project", new { id = projectId });
    }

    [HttpPost]
    public async Task<IActionResult> SubmitFinalVideo(Guid projectId, int videoId, ScriptStatus status,
        string clientComment)
    {
        if (!ModelState.IsValid)
            return BadRequest(InvalidArgumentsMessage);

        var project = await context.Projects
            .Include(p => p.Videos)
            .Include(p => p.ProjectComments)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
            return NotFound();

        var video = project.Videos.FirstOrDefault(v => v.VideoNumber == videoId);

        if (video == null)
            return NotFound("Video is not found!");

        video.Status = status;

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var comment = new ProjectComment
        {
            ProjectId = project.Id,
            UserId = userId,
            CommentText = clientComment,
            CommentStage = ProjectStage.VideoEditing,
            CreatedAt = DateTimeOffset.UtcNow,
            RelatedVideoNumber = video.VideoNumber
        };


        project.ProjectComments.Add(comment);

        if (project.Videos.All(v => v.Status == ScriptStatus.Approved))
            project.ProjectStage = ProjectStage.Complete;

        await context.SaveChangesAsync();

        return RedirectToAction("Details", "Project", new { id = projectId });
    }

    [HttpGet]
    public async Task<IActionResult> GenerateReport(Guid projectId)
    {
        if (!ModelState.IsValid)
            return BadRequest(InvalidArgumentsMessage);

        var project = await context.Projects
            .Where(p => p.Id == projectId)
            .FirstOrDefaultAsync();

        if (project == null)
            return NotFound();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Отчёт");

        worksheet.FirstCell().InsertTable([project]);
        worksheet.Column(5).Style.Alignment.WrapText = true;
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        var fileName = $"Отчет_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}