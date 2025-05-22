using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicStreamPlayer.Models;
using MusicStreamPlayer.Services;

namespace MusicStreamPlayer.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IAuthService authService, SignInManager<ApplicationUser> signInManager)
        {
            _authService = authService;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var (result, user) = await _authService.RegisterUserAsync(model);

                if (result.Succeeded)
                {
                    TempData["account"] = "Register";
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    TempData["account"] = error.Description;
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.LoginUserAsync(model);

                if (result.Succeeded)
                {
                    TempData["account"] = "Login";
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid login attempt.");
                TempData["account"] = "Invalid login attempt.";
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            TempData["account"] = "Logout";
            return RedirectToAction("Index", "Home");
        }
    }
}