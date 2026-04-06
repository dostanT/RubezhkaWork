using Microsoft.EntityFrameworkCore;

using repo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// Регистрация DbContext и сервиса
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=university.db"));

builder.Services.AddScoped<IUniversityDbService, UniversityDbService>();

var app = builder.Build();

// Применяем миграции при старте приложения
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
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
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();