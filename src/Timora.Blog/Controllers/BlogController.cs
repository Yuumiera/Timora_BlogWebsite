using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Timora.Blog.Data;
using Timora.Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Timora.Blog.Models.ViewModels;
using System.Linq;

namespace Timora.Blog.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<IdentityUser> _userManager;

        public BlogController(AppDbContext dbContext, IWebHostEnvironment env, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _env = env;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? category)
        {
            var query = _dbContext.Posts
                .Where(p => p.IsPublished)
                .Include(p => p.Category)
                .OrderByDescending(p => p.PublishedAt)
                .AsQueryable();

            Category? activeCategory = null;
            if (!string.IsNullOrWhiteSpace(category) && category != "tum-yazilar")
            {
                activeCategory = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Slug == category);
                if (activeCategory != null)
                {
                    query = query.Where(p => p.CategoryId == activeCategory.Id);
                }
            }

            var posts = await query.ToListAsync();

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("Ana Sayfa", Url.Action("Index", "Home")),
                new BreadcrumbItem("Blog", Url.Action("Index", "Blog"))
            };

            if (activeCategory != null)
            {
                breadcrumbs.Add(new BreadcrumbItem(activeCategory.Name, Url.Action("Index", "Blog", new { category = activeCategory.Slug })));
            }

            for (int i = 0; i < breadcrumbs.Count; i++)
            {
                bool isLast = i == breadcrumbs.Count - 1;
                breadcrumbs[i] = breadcrumbs[i] with { Active = isLast, Url = isLast ? null : breadcrumbs[i].Url };
            }

            ViewData["Breadcrumbs"] = breadcrumbs;

            return View(posts);
        }

        [HttpGet("blog/{slug}")]
        public async Task<IActionResult> Post(string slug)
        {
            var post = await _dbContext.Posts
                .Where(p => p.IsPublished)
                .Include(p => p.Author)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Slug == slug);
            if (post == null)
            {
                return NotFound();
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("Ana Sayfa", Url.Action("Index", "Home")),
                new BreadcrumbItem("Blog", Url.Action("Index", "Blog"))
            };

            if (post.Category != null)
            {
                breadcrumbs.Add(new BreadcrumbItem(post.Category.Name, Url.Action("Index", "Blog", new { category = post.Category.Slug })));
            }

            breadcrumbs.Add(new BreadcrumbItem(post.Title, null));
            for (int i = 0; i < breadcrumbs.Count; i++)
            {
                bool isLast = i == breadcrumbs.Count - 1;
                breadcrumbs[i] = breadcrumbs[i] with { Active = isLast, Url = isLast ? null : breadcrumbs[i].Url };
            }
            ViewData["Breadcrumbs"] = breadcrumbs;

            return View(post);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _dbContext.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewData["Breadcrumbs"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("Ana Sayfa", Url.Action("Index", "Home")),
                new BreadcrumbItem("Blog", Url.Action("Index", "Blog")),
                new BreadcrumbItem("Yeni Yazı", null, true)
            };
            return View(new PostCreateViewModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostCreateViewModel vm)
        {
            ViewData["Breadcrumbs"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("Ana Sayfa", Url.Action("Index", "Home")),
                new BreadcrumbItem("Blog", Url.Action("Index", "Blog")),
                new BreadcrumbItem("Yeni Yazı", null, true)
            };

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _dbContext.Categories.OrderBy(c => c.Name).ToListAsync();
                return View(vm);
            }

            string slug = GenerateSlug(vm.Title);
            // Ensure slug uniqueness
            int i = 1;
            string uniqueSlug = slug;
            while (await _dbContext.Posts.AnyAsync(p => p.Slug == uniqueSlug))
            {
                uniqueSlug = $"{slug}-{i++}";
            }

            string? coverUrl = null;
            if (vm.CoverImage != null && vm.CoverImage.Length > 0)
            {
                var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);
                var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(vm.CoverImage.FileName)}";
                var filePath = Path.Combine(uploadsRoot, fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await vm.CoverImage.CopyToAsync(stream);
                }
                coverUrl = $"/uploads/{fileName}";
            }

            var post = new Post
            {
                Title = vm.Title,
                Slug = uniqueSlug,
                Content = vm.Content,
                PublishedAt = DateTime.UtcNow,
                IsPublished = true,
                CategoryId = vm.CategoryId,
                CoverImageUrl = coverUrl
            };

            // Attach current user as author (ensure profile exists)
            var identityUserId = _userManager.GetUserId(User);
            if (!string.IsNullOrEmpty(identityUserId))
            {
                var profile = await _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.IdentityUserId == identityUserId);
                if (profile == null)
                {
                    var identityUser = await _userManager.GetUserAsync(User);
                    profile = new UserProfile
                    {
                        IdentityUserId = identityUserId,
                        FirstName = identityUser?.UserName ?? "Kullanıcı",
                        Email = identityUser?.Email
                    };
                    _dbContext.UserProfiles.Add(profile);
                    await _dbContext.SaveChangesAsync();
                }
                post.AuthorId = profile.Id;
            }

            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Post), new { slug = post.Slug });
        }

        private static string GenerateSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "post";
            string normalized = text.ToLowerInvariant();
            normalized = System.Text.RegularExpressions.Regex.Replace(normalized, "[\\u0300-\\u036f]", "");
            normalized = normalized.Replace('ı', 'i');
            normalized = normalized.Replace('ş', 's').Replace('ç', 'c').Replace('ğ', 'g').Replace('ö', 'o').Replace('ü', 'u');
            normalized = System.Text.RegularExpressions.Regex.Replace(normalized, "[^a-z0-9]+", "-");
            normalized = normalized.Trim('-');
            return string.IsNullOrEmpty(normalized) ? "post" : normalized;
        }
    }
}


