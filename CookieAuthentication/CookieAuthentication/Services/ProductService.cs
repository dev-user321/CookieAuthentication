using CookieAuthentication.Data;
using CookieAuthentication.Models;
using CookieAuthentication.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CookieAuthentication.Services
{
    public class ProductService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ProductVM> GetProductPageAsync()
        {
            var username = _httpContextAccessor.HttpContext.User.Identity.Name;

            var products = await _context.Products
                .Where(p => !p.SoftDelete)
                .Include(p => p.Images)
                .ToListAsync();

            var baskets = await _context.Baskets
                .Where(b => b.Username == username)
                .ToListAsync();

            var basketVMs = new List<BasketVM>();

            foreach (var basket in baskets)
            {
                var product = products.FirstOrDefault(p => p.Id == basket.ProductId);
                if (product == null) continue;

                basketVMs.Add(new BasketVM
                {
                    Id = basket.Id,
                    ProductName = product.Title,
                    ProductImage = product.Images.FirstOrDefault(i => i.IsMain)?.Image,
                    Count = basket.Count,
                    Price = product.Price * basket.Count
                });
            }

            return new ProductVM
            {
                Products = products,
                Baskets = basketVMs,
                UserName = username
            };
        }

        public async Task<Product?> GetProductDetailAsync(int id)
        {
            return await _context.Products
                .Where(p => !p.SoftDelete && p.Id == id)
                .Include(p => p.Images)
                .FirstOrDefaultAsync();
        }

        public async Task AddToBasketAsync(int productId)
        {
            var username = _httpContextAccessor.HttpContext.User.Identity.Name;

            var basket = await _context.Baskets
                .Where(b => !b.SoftDelete && b.ProductId == productId && b.Username == username)
                .FirstOrDefaultAsync();

            if (basket != null)
            {
                basket.Count++;
            }
            else
            {
                await _context.Baskets.AddAsync(new Basket
                {
                    Username = username,
                    ProductId = productId,
                    Count = 1
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveBasketItemAsync(int id)
        {
            var basket = await _context.Baskets.FindAsync(id);
            if (basket != null)
            {
                _context.Baskets.Remove(basket);
                await _context.SaveChangesAsync();
            }
        }
    }
}
