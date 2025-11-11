using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Timora.Blog.Data;
using Timora.Blog.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Timora.Blog.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileController(AppDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet("profile/{id:int}")]
        public async Task<IActionResult> Show(int id)
        {
            var profile = await _dbContext.UserProfiles
                .Include(u => u.Posts)
                .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (profile == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            bool isOwner = !string.IsNullOrEmpty(currentUserId) && profile.IdentityUserId == currentUserId;
            ViewBag.IsOwner = isOwner;

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("Ana Sayfa", Url.Action("Index", "Home")),
                new BreadcrumbItem("Yazarlar", Url.Action("Index", "Blog", new { category = "tum-yazilar" })),
                new BreadcrumbItem(profile.FullName, null)
            };
            for (int i = 0; i < breadcrumbs.Count; i++)
            {
                bool isLast = i == breadcrumbs.Count - 1;
                breadcrumbs[i] = breadcrumbs[i] with { Active = isLast, Url = isLast ? null : breadcrumbs[i].Url };
            }
            ViewData["Breadcrumbs"] = breadcrumbs;

            return View(profile);
        }
    }
}


