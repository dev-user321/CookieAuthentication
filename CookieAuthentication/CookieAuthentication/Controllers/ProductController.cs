using CookieAuthentication.Data;
using CookieAuthentication.Models;
using CookieAuthentication.Services;
using CookieAuthentication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookieAuthentication.Controllers
{
   
    [Authorize(Roles = "User , Admin")]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("products")]
        public async Task<IActionResult> Index()
        {
            var model = await _productService.GetProductPageAsync();
            return View(model);
        }

        [HttpPost("{slug}")]
        public async Task<IActionResult> Detail(string slug, int? id)
        {
            if (id == null) return BadRequest();

            var product = await _productService.GetProductDetailAsync(id.Value);
            if (product == null) return NotFound();

            return View(product);
        }

        public async Task<IActionResult> RemoveBasket(int id)
        {
            await _productService.RemoveBasketItemAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id == null) return BadRequest();

            await _productService.AddToBasketAsync(id.Value);
            return RedirectToAction("Index");
        }

    }
}
