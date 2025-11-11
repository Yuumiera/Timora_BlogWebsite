using System;

namespace Timora.Blog.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        // ASP.NET Core Identity linkage
        public string? IdentityUserId { get; set; }

        // Public profile fields
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string? Profession { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Interests { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();

        public string FullName => string.Join(" ", new[] { FirstName, LastName }.Where(x => !string.IsNullOrWhiteSpace(x)));
        public int? Age => BirthDate.HasValue ? (int)Math.Floor((DateTime.UtcNow - BirthDate.Value).TotalDays / 365.25) : null;
    }
}


