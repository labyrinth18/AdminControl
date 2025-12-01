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
        
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

        
            services.AddDbContext<AdminControlContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

        
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
        

        
            services.AddScoped<IAuthService, AuthService>();

        
            services.AddTransient<MainWindow>();
        

            services.AddTransient<LoginViewModel>();
            services.AddTransient<LoginWindow>(); 
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            
            
            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }
    }
}