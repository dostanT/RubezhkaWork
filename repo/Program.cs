using Microsoft.EntityFrameworkCore;
using repo.Data;
using repo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ========== ДОБАВЛЯЕМ НЕОБХОДИМЫЕ СЕРВИСЫ ==========
// HttpContextAccessor нужен для работы с сессиями в сервисах
builder.Services.AddHttpContextAccessor();

// Настройка сессий (ОБЯЗАТЕЛЬНО!)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Регистрация DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=university.db"));

// Регистрация сервисов
builder.Services.AddScoped<IUniversityDbService, UniversityDbService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// ========== ИНИЦИАЛИЗАЦИЯ БАЗЫ ДАННЫХ (ОДИН РАЗ) ==========
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Применяем миграции (создаёт таблицы)
    dbContext.Database.Migrate();
    
    // Заполняем тестовыми данными
    DbInitializer.Initialize(dbContext);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ========== ВАЖНО: Session ДО Authorization ==========
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();