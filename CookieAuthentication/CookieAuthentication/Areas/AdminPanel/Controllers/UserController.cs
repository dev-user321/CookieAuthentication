using CookieAuthentication.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using CookieAuthentication.Helper;
using Microsoft.AspNetCore.Authorization;

namespace CookieAuthentication.Areas.AdminPanel.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("AdminPanel")]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
  
        public UserController(AppDbContext context)
        {
            _context = context;
           
        }
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.Where(m => !m.SoftDelete).ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> ChangeRole(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Role = user.Role == "Admin" ? "User" : "Admin";
            await _context.SaveChangesAsync();

            

            return RedirectToAction("Index");
        }

    }
}
