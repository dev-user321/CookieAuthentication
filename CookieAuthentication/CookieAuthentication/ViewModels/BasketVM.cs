namespace CookieAuthentication.ViewModels
{
    public class BasketVM
    {
        public int Id { get; set; } 
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
        public int TotalCount { get; set; }
    }
}
