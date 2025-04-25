namespace CookieAuthentication.Models
{
    public class ProductImage : BaseEntity
    {
        public string Image {  get; set; }
        public int ProductId { get; set; }
        public bool IsMain { get; set; }
        public Product? Product { get; set; }
    }
}
