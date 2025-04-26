using CookieAuthentication.Data;
using CookieAuthentication.Helper;
using CookieAuthentication.Models;
using CookieAuthentication.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace CookieAuthentication.Services
{
    public class AccountService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(bool Success, string ErrorMessage)> LoginAsync(LoginVM loginVM)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginVM.Email);

            if (user == null || !PasswordHash.VerifyHashedPassword(user.Password, loginVM.Password))
                return (false, "Email və ya şifrə yanlışdır.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return (true, null);
        }

        public async Task<(bool Success, string ErrorMessage)> RegisterAsync(RegisterVM registerVM)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(m => m.Email == registerVM.Email || m.Username == registerVM.Username);

            if (existingUser != null)
                return (false, "Email və ya Username artıq mövcuddur.");

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
            return (true, null);
        }

        public async Task LogoutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
