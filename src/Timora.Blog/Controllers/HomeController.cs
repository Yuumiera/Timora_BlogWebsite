using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Timora.Blog.Data;
using Timora.Blog.Models;
using Timora.Blog.Models.ViewModels;

namespace Timora.Blog.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _dbContext;

    public HomeController(ILogger<HomeController> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _dbContext.Posts
            .Where(p => p.IsPublished)
            .Include(p => p.Category)
            .OrderByDescending(p => p.PublishedAt)
            .Take(9)
            .ToListAsync();

        // Get current culture for breadcrumbs
        var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
        var currentCulture = requestCultureFeature?.RequestCulture?.UICulture?.Name ?? "tr-TR";
        if (string.IsNullOrEmpty(currentCulture) || currentCulture == "tr") currentCulture = "tr-TR";
        var L = new Func<string, string>((key) => LanguageStrings.Get(key, currentCulture));

        ViewData["Breadcrumbs"] = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(L("Home"), Url.Action("Index", "Home"), true)
        };

        return View(posts);
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
