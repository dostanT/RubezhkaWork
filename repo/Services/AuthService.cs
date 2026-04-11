using Microsoft.EntityFrameworkCore;
using repo.Models;

namespace repo.Services
{
    public interface IAuthService
    {
        Task<bool> SignIn(Auth user);
        Task<bool> SignUp(Auth user);
        Task<bool> LogOut();
        Task<bool> IsExistingAsync(Auth auth);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthService> _logger; // Исправлен тип логгера

        public AuthService(ApplicationDbContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> IsExistingAsync(Auth auth)
        {
            // Исправлена опечатка: сравниваем login с auth.login
            // Используем StringComparison или просто проверяем наличие пользователя
            return await _context.Auth
                .AsNoTracking()
                .AnyAsync(a => a.login == auth.login);
        }

        public async Task<bool> SignIn(Auth user)
        {
            try
            {
                // Для входа проверяем и логин, и пароль
                var dbUser = await _context.Auth
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.login == user.login && a.password == user.password);

                if (dbUser != null)
                {
                    _logger.LogInformation("Пользователь {Login} успешно вошел", user.login);
                    return true;
                }

                _logger.LogWarning("Неудачная попытка входа для {Login}", user.login);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при SignIn");
                throw;
            }
        }

        public async Task<bool> SignUp(Auth user) 
        {
            try
            {
                // Проверяем, не занят ли логин
                if (await IsExistingAsync(user))
                {
                    _logger.LogWarning("Регистрация отклонена: логин {Login} занят", user.login);
                    return false;
                }

               
                if (user.login == "admin" && user.password == "admin")
                {
                    Auth nUser = new Auth
                    {
                        Id = user.Id,
                        login = user.login,
                        password = user.password,
                        isAdmin = true
                    };
                    _context.Auth.Add(nUser);
                }
                else
                {
                    _context.Auth.Add(user);
                }
                 await _context.SaveChangesAsync();

                _logger.LogInformation("Пользователь {Login} зарегистрирован", user.login);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при SignUp");
                throw;
            }
        }

        public async Task<bool> LogOut() 
        {
            try
            {
                // В JWT или простых сервисах логаут обычно делается на фронтенде (удаление токена).
                // Если сессии в БД, здесь должна быть логика их удаления.
                _logger.LogInformation("Выполнен выход (заглушка)");
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Ошибка при LogOut");
                throw;
            }
        }
    }
}