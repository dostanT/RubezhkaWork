using Microsoft.AspNetCore.Mvc;
using repo.Models;
using repo.Services;

namespace repo.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _service;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService service, ILogger<AuthController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // --- ВХОД (SIGN IN) ---

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(Auth model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _service.SignIn(model);

            if (result)
            {
                // Здесь обычно создаются Cookies или JWT токен
                _logger.LogInformation("Пользователь {Login} авторизован", model.login);
                return RedirectToAction("Index", "Home"); 
            }

            ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
            return View(model);
        }

        // --- РЕГИСТРАЦИЯ (SIGN UP) ---

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(Auth model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _service.SignUp(model);

            if (result)
            {
                _logger.LogInformation("Новый пользователь {Login} зарегистрирован", model.login);
                return RedirectToAction(nameof(SignIn));
            }

            ModelState.AddModelError("login", "Этот логин уже занят");
            return View(model);
        }

        // --- ВЫХОД (LOG OUT) ---

        [HttpPost] // Выход лучше делать POST-запросом для безопасности
        public async Task<IActionResult> LogOut()
        {
            await _service.LogOut();
            return RedirectToAction(nameof(SignIn));
        }

        // Страница по умолчанию (если нужна)
        public IActionResult Index()
        {
            return View();
        }
    }
}