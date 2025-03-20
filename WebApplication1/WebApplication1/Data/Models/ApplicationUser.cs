namespace WebApplication1.Data.Models;

/// <inheritdoc/>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    public required ICollection<Project> ManagedProjects { get; set; } = new List<Project>();
    public required ICollection<Project> ClientProjects { get; set; } = new List<Project>();
    public required ICollection<ProjectComment> UserComments { get; set; }
    public required ICollection<ApplicationUserRole> UserRoles { get; set; }
}