using CookieAuthentication.ViewModels;
using Microsoft.AspNetCore.Mvc;
using CookieAuthentication.Services;

namespace CookieAuthentication.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpGet]
        public IActionResult AccessDenied() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (loginVM == null) return BadRequest();

            var (success, errorMessage) = await _accountService.LoginAsync(loginVM);
            if (!success)
            {
                ModelState.AddModelError("", errorMessage);
                return View(loginVM);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (registerVM == null) return BadRequest();

            var (success, errorMessage) = await _accountService.RegisterAsync(registerVM);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                return View(registerVM);
            }

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
