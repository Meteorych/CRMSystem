using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

public class HomeController(ILogger<HomeController> logger) : Controller
{
    public IActionResult Index()
    {
        return Redirect("/Project");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}