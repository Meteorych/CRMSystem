using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Constants;
using WebApplication1.Data;

namespace WebApplication1.Controllers;

public class ProjectController(ApplicationDbContext context) : Controller
{
    private const string UserName = "UserName";

    public class CreateProjectDto
    {
        public required string ClientName { get; set; }
        public required string ManagerName { get; set; }

        [StringLength(100, MinimumLength = 2)]
        public required string Name { get; set; }
    }

    // GET: Project
    public async Task<IActionResult> Index(string? searchTerm)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        IQueryable<Project> query = context.Projects
            .Include(p => p.Manager)
            .Include(p => p.Client);

        if (!User.IsInRole(AuthConstants.AdminRole))
        {
            if (User.IsInRole(AuthConstants.ManagerRole))
            {
                query = query.Where(p => p.ManagerId == userId);
            }
            else if (User.IsInRole(AuthConstants.ClientRole))
            {
                query = query.Where(p => p.ClientId == userId);
            }
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm));
        }

        var projects = await query.ToListAsync();

        var activeProjects = projects
            .Where(p => p.ProjectStage != ProjectStage.Complete)
            .ToList();

        var completedProjects = projects
            .Where(p => p.ProjectStage == ProjectStage.Complete)
            .ToList();

        var model = new ProjectIndexViewModel
        {
            ActiveProjects = activeProjects,
            CompletedProjects = completedProjects
        };

        return View(model);
    }

    // GET: Project/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var project = await context.Projects
            .Include(p => p.Client)
            .Include(p => p.ProjectComments)
            .Include(p => p.Manager)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (project == null)
        {
            return NotFound();
        }

        return View(project);
    }

    // GET: Project/Create
    public IActionResult Create()
    {
        ViewData["ClientName"] = new SelectList(GetUsersByRole(AuthConstants.ClientRole), UserName, UserName);
        ViewData["ManagerName"] = new SelectList(GetUsersByRole(AuthConstants.ManagerRole), UserName, UserName);

        return View();
    }

    // POST: Project/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AuthConstants.AdminRole)]
    public async Task<IActionResult> Create(CreateProjectDto createProjectDto)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var client = await context.Users.FirstAsync(u => u.UserName == createProjectDto.ClientName);
        var manager = await context.Users.FirstAsync(u => u.UserName == createProjectDto.ManagerName);

        var maxProjectNumber = await context.Projects
            .AnyAsync()
            ? await context.Projects.MaxAsync(p => p.ProjectNumber)
            : 0;

        var project = new Project
        {
            Client = client,
            Manager = manager,
            Name = createProjectDto.Name,
            TechTask = new TechTask(),
            ProjectNumber = maxProjectNumber + 1,
            Id = Guid.CreateVersion7(),
            ProjectStage = ProjectStage.TechTask
        };

        context.Add(project);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: Project/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var project = await context.Projects
            .Include(pr => pr.Manager)
            .Include(pr => pr.Client)
            .FirstOrDefaultAsync(pr => pr.Id == id);

        if (project == null)
        {
            return NotFound();
        }

        ViewData["ClientName"] = new SelectList(GetUsersByRole(AuthConstants.ClientRole), UserName, UserName,
            project.Client.UserName);

        ViewData["ManagerName"] = new SelectList(GetUsersByRole(AuthConstants.ManagerRole), UserName, UserName,
            project.Manager.UserName);

        return View(project);
    }

    // POST: Project/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id,
        [Bind("Id,ProjectNumber,ManagerId,ClientId,Name,ProjectStage")]
        Project project)
    {
        if (id != project.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(project);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(project.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        project = await context.Projects
            .Include(pr => pr.Manager)
            .Include(pr => pr.Client)
            .FirstAsync(pr => pr.Id == id);

        ViewData["ClientName"] = new SelectList(GetUsersByRole(AuthConstants.ClientRole), UserName, UserName,
            project.Client.UserName);

        ViewData["ManagerName"] = new SelectList(GetUsersByRole(AuthConstants.ManagerRole), UserName, UserName,
            project.Manager.UserName);

        return View(project);
    }

    // GET: Project/Delete/5
    [Authorize(Roles = AuthConstants.AdminRole)]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var project = await context.Projects
            .Include(p => p.Client)
            .Include(p => p.Manager)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (project == null)
        {
            return NotFound();
        }

        return View(project);
    }

    // POST: Project/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AuthConstants.AdminRole)]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var project = await context.Projects.FindAsync(id);

        if (project != null)
        {
            context.Projects.Remove(project);
        }

        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = AuthConstants.ClientRole)]
    public async Task<IActionResult> SubmitTechTask(Guid projectId, [StringLength(1000, MinimumLength = 15)] string techTaskDescription, IFormFile? techTaskFile)
    {
        //TODO: Save file name
        if (!ModelState.IsValid)
            return BadRequest("Wrong arguments!");
        
        var project = await context.Projects
            .Include(p => p.TechTask)
            .Include(p => p.ProjectComments)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null) 
            return NotFound();

        project.ProjectComments.Add(new ProjectComment()
        {
            CommentStage = ProjectStage.TechTask,
            CommentText = techTaskDescription,
            UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
        });

        if (techTaskFile != null)
        {
            using var stream = new MemoryStream();
            await techTaskFile.CopyToAsync(stream);
            stream.Position = 0;

            project.TechTask.File = stream.ToArray();
        }

        await context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = projectId });
    }

    [HttpPost]
    [Authorize(Roles = AuthConstants.ClientRole)]
    public async Task<IActionResult> ApproveTechTask(Guid projectId)
    {
        if (!ModelState.IsValid)
            return BadRequest("Wrong arguments!");
        var project = await context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
            return NotFound();
        
        project.ProjectStage = ProjectStage.Script;
        await context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = projectId });
    }
    
    [HttpGet]
    public async Task<IActionResult> DownloadTechTaskFile(Guid projectId)
    {
            var project = await context.Projects
            .Include(p => p.TechTask)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project?.TechTask.File == null)
            return NotFound();

        var fileBytes = project.TechTask.File;
        const string contentType = "application/octet-stream";
        var fileName = $"TechTask_{project.ProjectNumber}.bin";

        return File(fileBytes, contentType, fileName);
    }

    private bool ProjectExists(Guid id)
    {
        return context.Projects.Any(e => e.Id == id);
    }

    private IQueryable<ApplicationUser> GetUsersByRole(string roleName)
    {
        return context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Where(u => u.UserRoles
                .Any(r => r.Role.Name == roleName));
    }
}