using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data.Models;

public class Project
{
    public Guid Id { get; set; }
    public Guid ManagerId { get; set; }
    public Guid ClientId { get; set; }
    
    [Column(TypeName = "nvarchar(32)")]
    public ProjectStage ProjectStage { get; set; }
    
    public required TechTask TechTask { get; set; }
    public required Video Video { get; set; }
    public required List<ProjectComment> ProjectComments { get; set; }
    public ApplicationUser Manager { get; set; }
    public ApplicationUser Client { get; set; }
}