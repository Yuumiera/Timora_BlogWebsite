using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Timora.Blog.Data;
using Timora.Blog.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace Timora.Blog.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ProfileController(AppDbContext dbContext, UserManager<IdentityUser> userManager, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _env = env;
        }

        [AllowAnonymous]
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

        [Authorize]
        [HttpGet("profile/edit")]
        public async Task<IActionResult> Edit()
        {
            var identityUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(identityUserId))
            {
                return Unauthorized();
            }

            var profile = await _dbContext.UserProfiles
                .FirstOrDefaultAsync(p => p.IdentityUserId == identityUserId);

            if (profile == null)
            {
                var identityUser = await _userManager.GetUserAsync(User);
                profile = new Models.UserProfile
                {
                    IdentityUserId = identityUserId,
                    FirstName = identityUser?.UserName ?? "Kullanıcı",
                    Email = identityUser?.Email
                };
                _dbContext.UserProfiles.Add(profile);
                await _dbContext.SaveChangesAsync();
            }

            var vm = new ProfileEditViewModel
            {
                Id = profile.Id,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                BirthDate = profile.BirthDate,
                Profession = profile.Profession,
                Gender = profile.Gender,
                Email = profile.Email,
                Phone = profile.Phone,
                Interests = profile.Interests,
                CurrentProfileImageUrl = profile.ProfileImageUrl
            };

            ViewData["Breadcrumbs"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("Ana Sayfa", Url.Action("Index", "Home")),
                new BreadcrumbItem("Profil", Url.Action("Show", "Profile", new { id = profile.Id })),
                new BreadcrumbItem("Profili Düzenle", null, true)
            };

            return View(vm);
        }

        [HttpPost("profile/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditViewModel vm)
        {
            var identityUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(identityUserId))
            {
                return Unauthorized();
            }

            var profile = await _dbContext.UserProfiles
                .FirstOrDefaultAsync(p => p.IdentityUserId == identityUserId);

            if (profile == null || profile.Id != vm.Id)
            {
                return Forbid();
            }

            ViewData["Breadcrumbs"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("Ana Sayfa", Url.Action("Index", "Home")),
                new BreadcrumbItem("Profil", Url.Action("Show", "Profile", new { id = profile.Id })),
                new BreadcrumbItem("Profili Düzenle", null, true)
            };

            if (!ModelState.IsValid)
            {
                vm.CurrentProfileImageUrl = profile.ProfileImageUrl;
                return View(vm);
            }

            profile.FirstName = vm.FirstName;
            profile.LastName = vm.LastName;
            profile.BirthDate = vm.BirthDate;
            profile.Profession = vm.Profession;
            profile.Gender = vm.Gender;
            profile.Email = vm.Email;
            profile.Phone = vm.Phone;
            profile.Interests = vm.Interests;

            if (vm.ProfileImage != null && vm.ProfileImage.Length > 0)
            {
                if (!string.IsNullOrEmpty(profile.ProfileImageUrl))
                {
                    var oldImagePath = Path.Combine(_env.WebRootPath, profile.ProfileImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", "profiles");
                if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);
                var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(vm.ProfileImage.FileName)}";
                var filePath = Path.Combine(uploadsRoot, fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await vm.ProfileImage.CopyToAsync(stream);
                }
                profile.ProfileImageUrl = $"/uploads/profiles/{fileName}";
            }

            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profiliniz başarıyla güncellendi!";
            return RedirectToAction(nameof(Show), new { id = profile.Id });
        }
    }
}


