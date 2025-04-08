namespace WebApplication1.Data.Models;

[Owned]
public class TechTask
{
    [MaxLength(128)]
    public string? FileName { get; set; }
    public byte[]? File { get; set; }
}