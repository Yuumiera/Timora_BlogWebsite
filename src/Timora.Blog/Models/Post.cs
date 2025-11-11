using System;

namespace Timora.Blog.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
        public bool IsPublished { get; set; } = false;

        // Relations
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public int? AuthorId { get; set; }
        public UserProfile? Author { get; set; }

        // Media
        public string? CoverImageUrl { get; set; }
    }
}


