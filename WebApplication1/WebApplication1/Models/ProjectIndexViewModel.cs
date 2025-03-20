namespace WebApplication1.Models;

public class ProjectIndexViewModel
{
    public List<Project> ActiveProjects { get; set; } = new();
    public List<Project> CompletedProjects { get; set; } = new();
}