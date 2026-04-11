using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using repo.Models;
using repo.Services;

namespace repo.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUniversityDbService _service;
        
        public AuthController(IUniversityDbService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            // Проверяем, есть ли пользователь в сессии
            var userId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) 
                return View(model);
            
            var (success, role, user) = await _service.LoginAsync(model.Login, model.Password);
            
            if (!success)
            {
                ModelState.AddModelError("", "Неверный логин или пароль");
                return View(model);
            }
            
            // Сохраняем данные в сессию
            if (user is Student s)
            {
                HttpContext.Session.SetString("UserId", s.Id.ToString());
                HttpContext.Session.SetString("UserRole", role);
                HttpContext.Session.SetString("UserName", $"{s.LastName} {s.FirstName}");
            }
            else if (user is Teacher t)
            {
                HttpContext.Session.SetString("UserId", t.Id.ToString());
                HttpContext.Session.SetString("UserRole", role);
                HttpContext.Session.SetString("UserName", $"{t.LastName} {t.FirstName}");
            }
                
            return RedirectToAction("Index", "Home");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _service.LogoutAsync();
            HttpContext.Session.Clear(); // Очищаем сессию
            return RedirectToAction("Login", "Auth");
        }
        
        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
    
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите логин")]
        [Display(Name = "Логин")]
        public string Login { get; set; } = null!;
        
        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = null!;
        
        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}