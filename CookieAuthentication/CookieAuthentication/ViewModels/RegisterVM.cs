using System.ComponentModel.DataAnnotations;

namespace CookieAuthentication.ViewModels
{
    public class RegisterVM
    {
        [Required]
        public string? FullName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifrələr uyğun deyil.")]
        public string? ConfirmPassword { get; set; }
    }
}
