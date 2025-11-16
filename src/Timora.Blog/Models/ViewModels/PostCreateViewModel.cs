using System.ComponentModel.DataAnnotations;

namespace Timora.Blog.Models.ViewModels
{
    public class PostCreateViewModel
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir kategori seçiniz.")]
        public int CategoryId { get; set; }

        public IFormFile? CoverImage { get; set; }
    }
}


