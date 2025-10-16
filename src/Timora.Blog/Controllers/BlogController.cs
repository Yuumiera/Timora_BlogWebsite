using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Timora.Blog.Data;

namespace Timora.Blog.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _dbContext;

        public BlogController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _dbContext.Posts
                .OrderByDescending(p => p.PublishedAt)
                .ToListAsync();
            return View(posts);
        }

        [HttpGet("blog/{slug}")]
        public async Task<IActionResult> Post(string slug)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Slug == slug);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }
    }
}


