namespace WebApplication1.Data.Models;

public class ApplicationUserRole : IdentityUserRole<Guid>
{
    public required ApplicationUser User { get; set; }
    public required ApplicationRole Role { get; set; }
}