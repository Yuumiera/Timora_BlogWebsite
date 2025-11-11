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

        [Required]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "En az bir görsel (kapak fotoğrafı) yüklemelisiniz.")]
        public IFormFile? CoverImage { get; set; }
    }
}


