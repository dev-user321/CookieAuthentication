using ShopApp.Helper;

namespace CookieAuthentication.Models
{
    public class Product :BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<ProductImage> Images { get; set; }
        public int Price { get; set; }
        public int Count {  get; set; }

        public string Slug => SlugHelper.GenerateSlug(Title);
    }
}
