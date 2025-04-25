using CookieAuthentication.Data;
using CookieAuthentication.Helper;
using CookieAuthentication.Models;
using CookieAuthentication.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;

namespace CookieAuthentication.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (loginVM == null) return BadRequest();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginVM.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Email və ya şifrə yanlışdır.");
                return View(loginVM);
            }

            bool isPasswordValid = PasswordHash.VerifyHashedPassword(user.Password, loginVM.Password);

            if (!isPasswordValid)
            {
                ModelState.AddModelError("", "Email və ya şifrə yanlışdır.");
                return View(loginVM);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (registerVM == null)
                return BadRequest();

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(m => m.Email == registerVM.Email || m.Username == registerVM.Username);

            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Email və ya Username artıq mövcuddur.");
                return View(registerVM);
            }
            string hashedPassword = PasswordHash.HashPassword(registerVM.Password);
            var user = new AppUser
            {
                FullName = registerVM.FullName,
                Email = registerVM.Email,
                Username = registerVM.Username,
                Password = hashedPassword,  
                Role = "User"
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index","Home");
        }

    }
}
