using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Timora.Blog.Models;

namespace Timora.Blog.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>(entity =>
            {
                entity.Property(p => p.Title).HasMaxLength(200).IsRequired();
                entity.Property(p => p.Slug).HasMaxLength(200).IsRequired();
                entity.HasIndex(p => p.Slug).IsUnique();
                entity.Property(p => p.CoverImageUrl).HasMaxLength(1024);

                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Posts)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(p => p.Author)
                    .WithMany(u => u.Posts)
                    .HasForeignKey(p => p.AuthorId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Name).HasMaxLength(100).IsRequired();
                entity.Property(c => c.Slug).HasMaxLength(120).IsRequired();
                entity.HasIndex(c => c.Slug).IsUnique();
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.Property(u => u.FirstName).HasMaxLength(100);
                entity.Property(u => u.LastName).HasMaxLength(100);
                entity.Property(u => u.Profession).HasMaxLength(150);
                entity.Property(u => u.Gender).HasMaxLength(50);
                entity.Property(u => u.Email).HasMaxLength(200);
                entity.Property(u => u.Phone).HasMaxLength(50);
                entity.Property(u => u.ProfileImageUrl).HasMaxLength(1024);
                entity.Property(u => u.Interests).HasMaxLength(300);
                entity.Property(u => u.IdentityUserId).HasMaxLength(450);
                entity.HasIndex(u => u.IdentityUserId);
            });

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Yaşam Tarzı ve Kişisel Gelişim", Slug = "yasam-tarzi-ve-kisisel-gelisim" },
                new Category { Id = 2, Name = "Yemek ve Beslenme", Slug = "yemek-ve-beslenme" },
                new Category { Id = 3, Name = "Seyahat ve Keşif", Slug = "seyahat-ve-kesif" },
                new Category { Id = 4, Name = "Moda ve Stil", Slug = "moda-ve-stil" },
                new Category { Id = 5, Name = "Teknoloji ve İnceleme", Slug = "teknoloji-ve-inceleme" },
                new Category { Id = 6, Name = "Sağlık ve Fitness", Slug = "saglik-ve-fitness" },
                new Category { Id = 7, Name = "Hobi ve Sanat", Slug = "hobi-ve-sanat" },
                new Category { Id = 8, Name = "Eğitim ve Kariyer", Slug = "egitim-ve-kariyer" },
                new Category { Id = 9, Name = "Edebiyat ve Kitap", Slug = "edebiyat-ve-kitap" },
                new Category { Id = 10, Name = "Ev ve Dekorasyon", Slug = "ev-ve-dekorasyon" },
                new Category { Id = 11, Name = "Tüm Yazılar", Slug = "tum-yazilar" }
            );
        }
    }
}


