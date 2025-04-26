using CookieAuthentication.Areas.AdminPanel.Services;
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
        private readonly ProductAdminService _service;

        public ProductController(ProductAdminService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _service.GetAllAsync();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM vm)
        {
            if (!ModelState.IsValid || vm.Photos == null || !vm.Photos.Any())
                return View(vm);

            await _service.CreateAsync(vm);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.GetUpdateVMAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductUpdateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            await _service.UpdateAsync(id, vm);
            return RedirectToAction(nameof(Index));
        }


    }
}
