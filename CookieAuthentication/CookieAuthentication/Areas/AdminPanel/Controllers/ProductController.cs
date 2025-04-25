using CookieAuthentication.Data;
using CookieAuthentication.Models;
using CookieAuthentication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookieAuthentication.Areas.AdminPanel.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("AdminPanel")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ProductController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Where(m => !m.SoftDelete)
                .Include(m => m.Images)
                .ToListAsync();
            return View(products);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM productCreateVM)
        {
            if (productCreateVM == null || productCreateVM.Photos == null || !productCreateVM.Photos.Any())
                return BadRequest("Məlumat və ya şəkil tapılmadı.");

            var productImages = new List<ProductImage>();

            foreach (var photo in productCreateVM.Photos)
            {
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                string filePath = Path.Combine(_env.WebRootPath, "img", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                var productImage = new ProductImage
                {
                    Image = fileName
                };

                productImages.Add(productImage);
            }

            if (productImages.FirstOrDefault() is ProductImage mainImage)
            {
                mainImage.IsMain = true;
            }

            await _context.ProductImages.AddRangeAsync(productImages);

            var product = new Product
            {
                Title = productCreateVM.Title,
                Description = productCreateVM.Description,
                Count = (int)productCreateVM.Count,
                Price = (int)productCreateVM.Price,
                Images = productImages
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            var product = await _context.Products
                .Include(m=>m.Images)
                .FirstOrDefaultAsync(x => x.Id == id);
            foreach(var oldImage in product.Images)
            {
                string oldPath = Path.Combine(_env.WebRootPath, "img", oldImage.Image);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath); 
                }

                _context.ProductImages?.Remove(oldImage);
            } 
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id) 
        {
            var product = await _context.Products.Where(m=>!m.SoftDelete)
                .Include(m=>m.Images)
                .FirstOrDefaultAsync(x=>x.Id == id);
            ProductUpdateVM productUpdateVM = new ProductUpdateVM()
            {
                Title = product.Title,
                Description = product.Description,
                Count = product.Count,
                Price = product.Price,
                ProductImages = product.Images,
            };

            return View(productUpdateVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, ProductUpdateVM productUpdateVM)
        {
            if (id == null) return BadRequest();

            var oldProduct = await _context.Products
                .Where(p => !p.SoftDelete)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (oldProduct == null) return NotFound();

            List<ProductImage> finalImages = new List<ProductImage>();

            if (productUpdateVM.Photos != null && productUpdateVM.Photos.Any())
            {
                foreach (var image in oldProduct.Images)
                {
                    string oldPath = Path.Combine(_env.WebRootPath, "img", image.Image);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }

                    _context.ProductImages.Remove(image);
                }

                foreach (var photo in productUpdateVM.Photos)
                {
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                    string filePath = Path.Combine(_env.WebRootPath, "img", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    finalImages.Add(new ProductImage
                    {
                        Image = fileName
                    });
                }

                if (finalImages.Any())
                {
                    finalImages.First().IsMain = true;
                }

                await _context.ProductImages.AddRangeAsync(finalImages);
            }
            else
            {
                finalImages = oldProduct.Images.ToList();
            }

            oldProduct.Title = productUpdateVM.Title;
            oldProduct.Description = productUpdateVM.Description;
            oldProduct.Price = (int)productUpdateVM.Price;
            oldProduct.Count = (int)productUpdateVM.Count;
            oldProduct.Images = finalImages;
            oldProduct.CreateTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

    }
}
