using CookieAuthentication.Data;
using CookieAuthentication.Models;
using CookieAuthentication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookieAuthentication.Controllers
{
   
    [Authorize(Roles = "User , Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("products")]
        public async Task<IActionResult> Index()
        {
         
            var products = await _context.Products
                .Where(m => !m.SoftDelete)
                .Include(m => m.Images)
                .ToListAsync();

            var baskets = await _context.Baskets.Where(m=>m.Username == User.Identity.Name).ToListAsync();

            List<BasketVM> basketVMs = new List<BasketVM>();
            foreach (var basket in baskets)
            {
                var product = products.FirstOrDefault(x => x.Id == basket.ProductId);
                if (product == null) continue;

                BasketVM basketVM = new BasketVM()
                {
                    Id = basket.Id,
                    ProductName = product.Title,
                    ProductImage = product.Images.FirstOrDefault(m => m.IsMain)?.Image,
                    Count = basket.Count,
                    Price = product.Price*basket.Count
                     
                };
                basketVMs.Add(basketVM);
            }

            ProductVM productVM = new ProductVM()
            {
                Products = products,
                Baskets = basketVMs,
                UserName = User.Identity.Name
            };

            return View(productVM);
        }

        [HttpPost("{slug}")]
        public async Task<IActionResult> Detail(string slug,int? id)
        {
            
                if (id == null) return BadRequest();

                var product = await _context.Products
                    .Where(m => !m.SoftDelete)
                    .Include(m => m.Images)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (product == null) return NotFound();

                return View(product);
                     
        }



        public async Task<IActionResult> RemoveBasket(int id)
        {
            var basket = await _context.Baskets.FindAsync(id);
            _context.Baskets.Remove(basket);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddBasket(int? id)
        {
            string userName = User.Identity.Name;
            var basket = await _context.Baskets.Where(m => !m.SoftDelete).FirstOrDefaultAsync(m => m.ProductId == id && m.Username == userName); 
            if(basket != null)
            {
                basket.Count++;

            }
            else
            {
                var newBasket = new Basket()
                {
                    Username = User.Identity?.Name,
                    ProductId = id,
                    Count = 1
                };
                await _context.Baskets.AddAsync(newBasket);

            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
         
        }

    }
}
