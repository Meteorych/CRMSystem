using System.Security.Claims;
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

        var project = await context.Projects.Include(p => p.Video).FirstOrDefaultAsync(p => p.Id == projectId);
        var maxVideoNumber = await context.Projects.MaxAsync(p => p.Video != null ? p.Video.VideoNumber : 0);

        if (project == null || project.Video != null)
            return BadRequest("Invalid project state!");

        project.Video = new Video
        {
            VideoNumber = maxVideoNumber + 1,
            Script = script,
            Reference = new Uri(reference),
            Status = ScriptStatus.Correction // Starts with Correction
        };

        await context.SaveChangesAsync();

        return RedirectToAction("Details", "Project", new { id = projectId });
    }

    [HttpPost]
    [Authorize(Roles = AuthConstants.ManagerRole)]
    public async Task<IActionResult> UpdateVideo(Guid projectId, string script, string reference)
    {
        if (!ModelState.IsValid)
            return BadRequest(InvalidArgumentsMessage);

        var project = await context.Projects.Include(p => p.Video).FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null || project.Video == null)
            return BadRequest("Invalid project!");

        project.Video.Script = script;
        project.Video.Reference = new Uri(reference);
        project.Video.Status = ScriptStatus.Correction; // Still correction until client approves

        await context.SaveChangesAsync();

        return RedirectToAction("Details", "Project", new { id = projectId });
    }

    [HttpPost]
    [Authorize(Roles = AuthConstants.ClientRole)]
    public async Task<IActionResult> SubmitScript(Guid projectId, ScriptStatus status, string clientComment)
    {
        if (!ModelState.IsValid)
            return BadRequest(InvalidArgumentsMessage);

        var project = await context.Projects.Include(p => p.Video).FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null || project.Video == null)
            return BadRequest("Invalid project!");

        project.Video.Status = status;

        project.ProjectComments.Add(new ProjectComment
        {
            UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            CommentText = clientComment,
            CommentStage = ProjectStage.Script
        });

        if (status == ScriptStatus.Approved)
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
        
        var project = await context.Projects.FindAsync(projectId);

        if (project == null || project.ProjectStage != ProjectStage.Filming)
            return NotFound();

        project.ProjectStage = ProjectStage.VideoEditing;
        await context.SaveChangesAsync();

        return RedirectToAction("Details", "Project", new { id = projectId });
    }
}