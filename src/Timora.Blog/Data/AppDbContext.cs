using Microsoft.EntityFrameworkCore;
using Timora.Blog.Models;

namespace Timora.Blog.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts => Set<Post>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>(entity =>
            {
                entity.Property(p => p.Title).HasMaxLength(200).IsRequired();
                entity.Property(p => p.Slug).HasMaxLength(200).IsRequired();
                entity.HasIndex(p => p.Slug).IsUnique();
            });
        }
    }
}


