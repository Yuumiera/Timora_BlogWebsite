using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Timora.Blog.Data;
using Timora.Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Timora.Blog.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Localization;

namespace Timora.Blog.Controllers
{
    [Route("[controller]")]
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

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string? q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return RedirectToAction(nameof(Index));
            }

            var query = _dbContext.Posts
                .Where(p => p.IsPublished)
                .Include(p => p.Category)
                .Where(p => p.Title.Contains(q))
                .OrderByDescending(p => p.PublishedAt)
                .AsQueryable();

            var posts = await query.ToListAsync();

            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var currentCulture = requestCultureFeature?.RequestCulture?.UICulture?.Name ?? "tr-TR";
            if (string.IsNullOrEmpty(currentCulture) || currentCulture == "tr") currentCulture = "tr-TR";
            var L = new Func<string, string>((key) => LanguageStrings.Get(key, currentCulture));

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(L("Home"), Url.Action("Index", "Home")),
                new BreadcrumbItem(L("Blog"), Url.Action("Index", "Blog")),
                new BreadcrumbItem($"{L("Search Results")}: {q}", null, true)
            };

            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["SearchQuery"] = q;
            ViewData["ResultCount"] = posts.Count;

            return View("Search", posts);
        }

        [HttpGet("Index")]
        [HttpGet]
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

            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var currentCulture = requestCultureFeature?.RequestCulture?.UICulture?.Name ?? "tr-TR";
            if (string.IsNullOrEmpty(currentCulture) || currentCulture == "tr") currentCulture = "tr-TR";
            var L = new Func<string, string>((key) => LanguageStrings.Get(key, currentCulture));

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(L("Home"), Url.Action("Index", "Home")),
                new BreadcrumbItem(L("Blog"), Url.Action("Index", "Blog"))
            };

            if (activeCategory != null)
            {
                var categoryName = L(activeCategory.Slug);
                breadcrumbs.Add(new BreadcrumbItem(categoryName, Url.Action("Index", "Blog", new { category = activeCategory.Slug })));
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

            // Get current culture for breadcrumbs
            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var currentCulture = requestCultureFeature?.RequestCulture?.UICulture?.Name ?? "tr-TR";
            if (string.IsNullOrEmpty(currentCulture) || currentCulture == "tr") currentCulture = "tr-TR";
            var L = new Func<string, string>((key) => LanguageStrings.Get(key, currentCulture));

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(L("Home"), Url.Action("Index", "Home")),
                new BreadcrumbItem(L("Blog"), Url.Action("Index", "Blog"))
            };

            if (post.Category != null)
            {
                var categoryName = L(post.Category.Slug);
                breadcrumbs.Add(new BreadcrumbItem(categoryName, Url.Action("Index", "Blog", new { category = post.Category.Slug })));
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
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var currentCulture = requestCultureFeature?.RequestCulture?.UICulture?.Name ?? "tr-TR";
            if (string.IsNullOrEmpty(currentCulture) || currentCulture == "tr") currentCulture = "tr-TR";
            var L = new Func<string, string>((key) => LanguageStrings.Get(key, currentCulture));

            ViewBag.Categories = await _dbContext.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewData["Breadcrumbs"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(L("Home"), Url.Action("Index", "Home")),
                new BreadcrumbItem(L("Blog"), Url.Action("Index", "Blog")),
                new BreadcrumbItem(L("New Post"), null, true)
            };
            return View(new PostCreateViewModel());
        }

        [Authorize]
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostCreateViewModel vm)
        {
            // Get current culture for breadcrumbs
            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var currentCulture = requestCultureFeature?.RequestCulture?.UICulture?.Name ?? "tr-TR";
            if (string.IsNullOrEmpty(currentCulture) || currentCulture == "tr") currentCulture = "tr-TR";
            var L = new Func<string, string>((key) => LanguageStrings.Get(key, currentCulture));

            ViewData["Breadcrumbs"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(L("Home"), Url.Action("Index", "Home")),
                new BreadcrumbItem(L("Blog"), Url.Action("Index", "Blog")),
                new BreadcrumbItem(L("New Post"), null, true)
            };

            if (vm.CoverImage == null || vm.CoverImage.Length == 0)
            {
                ModelState.AddModelError(nameof(vm.CoverImage), "Kapak fotoğrafı zorunludur.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _dbContext.Categories.OrderBy(c => c.Name).ToListAsync();
                return View(vm);
            }

            string slug = GenerateSlug(vm.Title);
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
            
            TempData["SuccessMessage"] = "Yazınız başarıyla yayınlandı!";
            return RedirectToAction(nameof(Post), new { slug = post.Slug });
        }

        [Authorize]
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _dbContext.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            var identityUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(identityUserId))
            {
                return Unauthorized();
            }

            var profile = await _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.IdentityUserId == identityUserId);
            if (profile == null || post.AuthorId != profile.Id)
            {
                return Forbid();
            }

            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var currentCulture = requestCultureFeature?.RequestCulture?.UICulture?.Name ?? "tr-TR";
            if (string.IsNullOrEmpty(currentCulture) || currentCulture == "tr") currentCulture = "tr-TR";
            var L = new Func<string, string>((key) => LanguageStrings.Get(key, currentCulture));

            ViewBag.Categories = await _dbContext.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewData["Breadcrumbs"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(L("Home"), Url.Action("Index", "Home")),
                new BreadcrumbItem(L("Blog"), Url.Action("Index", "Blog")),
                new BreadcrumbItem(L("Edit"), null, true)
            };

            var vm = new PostCreateViewModel
            {
                Title = post.Title,
                Content = post.Content,
                CategoryId = post.CategoryId ?? 0
            };

            ViewBag.PostId = post.Id;
            ViewBag.CurrentCoverImage = post.CoverImageUrl;

            return View(vm);
        }

        [Authorize]
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PostCreateViewModel vm)
        {
            var post = await _dbContext.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            var identityUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(identityUserId))
            {
                return Unauthorized();
            }

            var profile = await _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.IdentityUserId == identityUserId);
            if (profile == null || post.AuthorId != profile.Id)
            {
                return Forbid();
            }

            // Get current culture for breadcrumbs
            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var currentCulture = requestCultureFeature?.RequestCulture?.UICulture?.Name ?? "tr-TR";
            if (string.IsNullOrEmpty(currentCulture) || currentCulture == "tr") currentCulture = "tr-TR";
            var L = new Func<string, string>((key) => LanguageStrings.Get(key, currentCulture));

            ViewData["Breadcrumbs"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(L("Home"), Url.Action("Index", "Home")),
                new BreadcrumbItem(L("Blog"), Url.Action("Index", "Blog")),
                new BreadcrumbItem(L("Edit"), null, true)
            };

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _dbContext.Categories.OrderBy(c => c.Name).ToListAsync();
                ViewBag.PostId = post.Id;
                ViewBag.CurrentCoverImage = post.CoverImageUrl;
                return View(vm);
            }

            bool titleChanged = post.Title != vm.Title;
            string oldSlug = post.Slug;

            // Update post
            post.Title = vm.Title;
            post.Content = vm.Content;
            post.CategoryId = vm.CategoryId;

            if (titleChanged)
            {
                string slug = GenerateSlug(vm.Title);
                int i = 1;
                string uniqueSlug = slug;
                while (await _dbContext.Posts.AnyAsync(p => p.Slug == uniqueSlug && p.Id != id))
                {
                    uniqueSlug = $"{slug}-{i++}";
                }
                post.Slug = uniqueSlug;
            }

            if (vm.CoverImage != null && vm.CoverImage.Length > 0)
            {
                if (!string.IsNullOrEmpty(post.CoverImageUrl))
                {
                    var oldImagePath = Path.Combine(_env.WebRootPath, post.CoverImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);
                var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(vm.CoverImage.FileName)}";
                var filePath = Path.Combine(uploadsRoot, fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await vm.CoverImage.CopyToAsync(stream);
                }
                post.CoverImageUrl = $"/uploads/{fileName}";
            }

            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Yazınız başarıyla güncellendi!";
            return RedirectToAction(nameof(Post), new { slug = post.Slug });
        }

        [Authorize]
        [HttpPost("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _dbContext.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            var identityUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(identityUserId))
            {
                return Unauthorized();
            }

            var profile = await _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.IdentityUserId == identityUserId);
            if (profile == null || post.AuthorId != profile.Id)
            {
                return Forbid();
            }

            if (!string.IsNullOrEmpty(post.CoverImageUrl))
            {
                var imagePath = Path.Combine(_env.WebRootPath, post.CoverImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _dbContext.Posts.Remove(post);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Yazınız başarıyla silindi!";
            return RedirectToAction(nameof(Index));
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


