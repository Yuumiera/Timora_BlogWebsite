using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Timora.Blog.Data;

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
            return View(categories);
        }
    }
}


