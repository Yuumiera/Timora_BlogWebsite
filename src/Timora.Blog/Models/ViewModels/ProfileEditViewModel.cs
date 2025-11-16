using System.ComponentModel.DataAnnotations;

namespace Timora.Blog.Models.ViewModels
{
    public class ProfileEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [Display(Name = "Ad")]
        [MaxLength(100, ErrorMessage = "Ad en fazla 100 karakter olabilir.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [Display(Name = "Soyad")]
        [MaxLength(100, ErrorMessage = "Soyad en fazla 100 karakter olabilir.")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Doğum Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Meslek")]
        [MaxLength(100, ErrorMessage = "Meslek en fazla 100 karakter olabilir.")]
        public string? Profession { get; set; }

        [Display(Name = "Cinsiyet")]
        [MaxLength(50, ErrorMessage = "Cinsiyet en fazla 50 karakter olabilir.")]
        public string? Gender { get; set; }

        [Display(Name = "E-posta")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [MaxLength(200, ErrorMessage = "E-posta en fazla 200 karakter olabilir.")]
        public string? Email { get; set; }

        [Display(Name = "Telefon")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [MaxLength(20, ErrorMessage = "Telefon en fazla 20 karakter olabilir.")]
        public string? Phone { get; set; }

        [Display(Name = "İlgi Alanları ve Hobiler")]
        [MaxLength(300, ErrorMessage = "İlgi alanları en fazla 300 karakter olabilir.")]
        public string? Interests { get; set; }

        [Display(Name = "Profil Fotoğrafı")]
        public IFormFile? ProfileImage { get; set; }

        public string? CurrentProfileImageUrl { get; set; }
    }
}

