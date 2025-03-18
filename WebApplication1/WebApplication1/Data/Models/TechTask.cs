namespace WebApplication1.Data.Models;

[Owned]
public class TechTask
{
    public Uri? TechTaskUrl { get; set; }
    public byte[]? File { get; set; }
}