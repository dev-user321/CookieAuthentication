using CookieAuthentication.Models;

namespace CookieAuthentication.ViewModels
{
    public class ProductVM
    {
        public List<Product> Products { get; set; } 
        public List<BasketVM> Baskets { get; set; }
        public string? UserName { get; set; }
    }
}
