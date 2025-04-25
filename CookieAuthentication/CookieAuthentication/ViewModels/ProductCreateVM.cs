using System.ComponentModel.DataAnnotations;

namespace CookieAuthentication.ViewModels
{
    public class ProductCreateVM
    {
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public int? Price { get; set; }
        [Required]
        public int? Count { get; set; }
        public List<IFormFile>? Photos { get; set; }

    }
}
