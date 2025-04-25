namespace CookieAuthentication.Models
{
    public class Basket : BaseEntity
    {
        public string? Username { get; set; }
        public int? ProductId { get; set; }
        public int Count { get; set; }
    }
}
