using AdminControl.BLL.Interfaces;
using AdminControl.BLL.Services;
using AdminControl.DAL;
using AdminControl.DALEF.Concrete;
using AdminControl.WPF.ViewModels;
using AdminControl.WPF.Views;
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
        public IServiceProvider ServiceProvider { get; private set; } = null!;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Налаштування конфігурації (appsettings.json)
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            // Підключення БД
            services.AddDbContext<AdminControlContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // Реєстрація репозиторіїв (DAL)
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserBankCardRepository, UserBankCardRepository>();
            services.AddScoped<IActionTypeRepository, ActionTypeRepository>();
            services.AddScoped<IAdminActionLogRepository, AdminActionLogRepository>();

            // Реєстрація сервісів (BLL)
            services.AddScoped<IAuthService, AuthService>();

            // Реєстрація ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<AddEditUserViewModel>();

            // Реєстрація вікон
            services.AddTransient<LoginWindow>();
            services.AddTransient<Views.MainWindow>();
            services.AddTransient<AddEditUserWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Стартуємо з вікна Логіну
            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }
    }
}
