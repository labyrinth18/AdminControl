using AdminControl.BLL.Interfaces;
using AdminControl.BLL.Services;
using AdminControl.WPF.Views;
using AdminControl.DAL;
using AdminControl.DALEF.Concrete;
using AdminControl.WPF.ViewModels; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

namespace AdminControl.WPF
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // 1. Конфігурація (appsettings.json)
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            // 2. База даних
            services.AddDbContext<AdminControlContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // 3. Репозиторії (DAL)
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            // services.AddScoped<IUserBankCardRepository, UserBankCardRepository>(); // Якщо потрібно

            // 4. Бізнес-логіка (BLL)
            services.AddScoped<IAuthService, AuthService>();

            // 5. Вікна та ViewModel (WPF)
            services.AddTransient<MainWindow>();
            // services.AddTransient<MainViewModel>(); // Пізніше створимо

            services.AddTransient<LoginViewModel>();
            services.AddTransient<LoginWindow>(); // Створимо це вікно на наступному кроці!
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // ЗАПУСК: Показуємо вікно входу
            // Якщо LoginWindow ще не створено, цей рядок поки не працюватиме.
            // Ми виправимо це на наступному кроці.
            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }
    }
}