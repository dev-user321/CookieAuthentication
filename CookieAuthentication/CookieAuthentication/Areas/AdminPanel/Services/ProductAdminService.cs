using CookieAuthentication.Data;
using CookieAuthentication.Models;
using CookieAuthentication.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CookieAuthentication.Areas.AdminPanel.Services
{
    public class ProductAdminService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductAdminService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Where(p => !p.SoftDelete)
                .Include(p => p.Images)
                .ToListAsync();
        }

        public async Task CreateAsync(ProductCreateVM vm)
        {
            var images = new List<ProductImage>();

            foreach (var photo in vm.Photos)
            {
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                string path = Path.Combine(_env.WebRootPath, "img", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                images.Add(new ProductImage { Image = fileName });
            }

            if (images.FirstOrDefault() is ProductImage mainImg)
                mainImg.IsMain = true;

            await _context.ProductImages.AddRangeAsync(images);

            var product = new Product
            {
                Title = vm.Title,
                Description = vm.Description,
                Count = (int)vm.Count,
                Price = (int)vm.Price,
                Images = images
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return;

            foreach (var img in product.Images)
            {
                string path = Path.Combine(_env.WebRootPath, "img", img.Image);
                if (File.Exists(path)) File.Delete(path);
            }

            _context.ProductImages.RemoveRange(product.Images);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<ProductUpdateVM?> GetUpdateVMAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return null;

            return new ProductUpdateVM
            {
                Title = product.Title,
                Description = product.Description,
                Count = product.Count,
                Price = product.Price,
                ProductImages = product.Images
            };
        }

        public async Task UpdateAsync(int id, ProductUpdateVM vm)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return;

            List<ProductImage> newImages = product.Images.ToList();

            if (vm.Photos != null && vm.Photos.Any())
            {
                foreach (var img in product.Images)
                {
                    string path = Path.Combine(_env.WebRootPath, "img", img.Image);
                    if (File.Exists(path)) File.Delete(path);
                }

                _context.ProductImages.RemoveRange(product.Images);
                newImages.Clear();

                foreach (var photo in vm.Photos)
                {
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                    string filePath = Path.Combine(_env.WebRootPath, "img", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    newImages.Add(new ProductImage { Image = fileName });
                }

                if (newImages.Any())
                    newImages.First().IsMain = true;

                await _context.ProductImages.AddRangeAsync(newImages);
            }
           

            product.Title = vm.Title;
            product.Description = vm.Description;
            product.Count = (int)vm.Count;
            product.Price = (int)vm.Price;
            product.Images = newImages;
            product.CreateTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

    }
}
