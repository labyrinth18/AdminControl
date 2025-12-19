using AdminControl.BLL.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdminControl.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            // Використовуємо метод з вашого існуючого AuthService
            // Припускаємо, що він повертає true, якщо користувач валідний
            // Або повертає UserDto, якщо успішно

            // ПРИКЛАД (підлаштуйте під ваш AuthService):
            // var isValid = await _authService.ValidateUserAsync(login, password);

            // Емуляція для прикладу:
            bool isValid = true;

            if (isValid)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, login),
                    new Claim(ClaimTypes.Role, "Admin") // Роль має йти з БД
                };

                var id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));

                return RedirectToAction("Index", "Home");
            }

            _logger.LogWarning("Невдала спроба входу для користувача: {Login}", login);
            ModelState.AddModelError("", "Невірний логін або пароль");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            // Логування спроби доступу без прав
            _logger.LogWarning("Доступ заборонено (Access Denied) для користувача: {User}", User.Identity?.Name ?? "Anonymous");
            return View();
        }
    }
}