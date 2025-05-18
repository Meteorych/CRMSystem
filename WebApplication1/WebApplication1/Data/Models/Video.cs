using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data.Models;

[Owned]
public class Video
{
    public int VideoNumber { get; set; }
    
    [MaxLength(10000)]
    public required string Script { get; set; }
    public required Uri Reference { get; set; }
    public Uri? VideoUri { get; set; }
    
    [Column(TypeName = "nvarchar(16)")]
    public required ScriptStatus Status { get; set; }
}