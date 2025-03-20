using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Constants;
using WebApplication1.Data;

namespace WebApplication1.Controllers;

[Authorize(Roles = AuthConstants.AdminRole)]
public class UserController(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager) : Controller
{
    public async Task<ActionResult> Index()
    {
        var roles = await roleManager.Roles.ToListAsync();
        ViewBag.Roles = roles.SkipWhile(r => r.Name == AuthConstants.AdminRole);

        return View(await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Where(u => u.UserRoles.All(ur => ur.Role.Name != AuthConstants.AdminRole))
            .ToListAsync());
    }

    public async Task<ActionResult> SetUserRole(Guid id, string role)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var user = await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        var roles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, roles);

        await userManager.AddToRoleAsync(user, role);

        return RedirectToAction("Index");
    }
}