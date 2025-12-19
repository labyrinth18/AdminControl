using AdminControl.BLL.Interfaces;
using AdminControl.BLL.Services;
using AdminControl.DAL;
using AdminControl.DALEF.Concrete;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Налаштування контексту БД (рядок підключення в appsettings.json)
builder.Services.AddDbContext<AdminControlContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Реєстрація Repositories (DAL)
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
// ... додайте інші репозиторії за потреби

// 3. Реєстрація Services (BLL)
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// 4. AutoMapper (шукає профілі в збірці WebApp або можна вказати конкретну)
builder.Services.AddAutoMapper(typeof(Program));
// Якщо профілі у вас в DALEF або BLL, додайте: typeof(AdminControl.BLL.Services.RoleService).Assembly

// 5. Аутентифікація (Cookies)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Обов'язково перед Authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();