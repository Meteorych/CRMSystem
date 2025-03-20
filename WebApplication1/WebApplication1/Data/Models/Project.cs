using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data.Models;

[Index(nameof(ProjectNumber), IsUnique = true)]
public class Project
{
    public Guid Id { get; set; }
    
    public int ProjectNumber { get; set; }
    public Guid ManagerId { get; set; }
    public Guid ClientId { get; set; }
    
    [MaxLength(250)]
    public required string Name { get; set; }
    
    [Column(TypeName = "nvarchar(32)")]
    public ProjectStage ProjectStage { get; set; }
    
    public required TechTask TechTask { get; set; }
    public Video? Video { get; set; }
    public List<ProjectComment> ProjectComments { get; set; } = [];
    
    public required ApplicationUser Manager { get; set; }
    public required ApplicationUser Client { get; set; }
}