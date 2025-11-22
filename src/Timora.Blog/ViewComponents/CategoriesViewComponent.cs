using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Timora.Blog.Data;
using Timora.Blog.Models.ViewModels;

namespace Timora.Blog.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly AppDbContext _dbContext;

        public CategoriesViewComponent(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _dbContext.Categories
                .OrderBy(c => c.Id)
                .ToListAsync();
            
            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var currentCulture = requestCultureFeature?.RequestCulture?.UICulture?.Name ?? "tr-TR";
            if (string.IsNullOrEmpty(currentCulture) || currentCulture == "tr") currentCulture = "tr-TR";
            
            ViewBag.CurrentCulture = currentCulture;
            
            return View(categories);
        }
    }
}


