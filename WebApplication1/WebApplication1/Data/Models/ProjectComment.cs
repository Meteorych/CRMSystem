namespace WebApplication1.Data.Models;

public class ProjectComment
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public ProjectStage CommentStage { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int? RelatedVideoNumber { get; set; }
    
    [MaxLength(1000)]
    public required string CommentText { get; set; }
    public ApplicationUser User { get; set; } = null!;
}