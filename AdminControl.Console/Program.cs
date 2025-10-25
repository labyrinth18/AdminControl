using AdminControl.DAL;
using AdminControl.DALEF.Concrete;
using AdminControl.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    private static IRoleRepository _roleRepository = null!;
    private static IUserRepository _userRepository = null!;
    private static IUserBankCardRepository _userBankCardRepository = null!;

    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        using (var scope = host.Services.CreateScope())
        {
            _roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
            _userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            _userBankCardRepository = scope.ServiceProvider.GetRequiredService<IUserBankCardRepository>();
            await RunMainMenu();
        }
    }

    private static async Task RunMainMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("======= ГОЛОВНЕ МЕНЮ =======");
            Console.WriteLine("1. Управлiння ролями");
            Console.WriteLine("2. Управлiння користувачами");
            Console.WriteLine("---------------------------");
            Console.WriteLine("0. Вихiд з програми");
            Console.WriteLine("===========================");
            Console.Write("Ваш вибiр: ");
            string choice = Console.ReadLine() ?? "";
            switch (choice)
            {
                case "1": await RunRolesMenu(); break;
                case "2": await RunUsersMenu(); break;
                case "0": return;
                default: ShowPlaceholder("Невiрний вибiр. Спробуйте ще раз."); break;
            }
        }
    }

    private static async Task RunRolesMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("======= МЕНЮ: Управлiння ролями =======");
            Console.WriteLine("1. Показати всi ролi");
            Console.WriteLine("2. Додати нову роль");
            Console.WriteLine("3. Оновити роль");
            Console.WriteLine("4. Видалити роль");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("0. Повернутися до головного меню");
            Console.WriteLine("======================================");
            Console.Write("Ваш вибiр: ");
            string choice = Console.ReadLine() ?? "";
            switch (choice)
            {
                case "1": await ShowAllRoles(waitForInput: true); break; 
                case "2": await AddNewRole(); break;
                case "3": await UpdateExistingRole(); break;
                case "4": await DeleteExistingRole(); break;
                case "0": return;
                default: ShowPlaceholder("Невiрний вибiр. Спробуйте ще раз."); break;
            }
        }
    }

    private static async Task ShowAllRoles(bool waitForInput = false)
    {
        Console.WriteLine("--- Список всiх ролей ---");
        var roles = await _roleRepository.GetAllRolesAsync();
        if (roles.Any()) { foreach (var role in roles) { Console.WriteLine($"ID: {role.RoleID}, Назва: {role.RoleName}"); } }
        else { Console.WriteLine("Список ролей порожнiй."); }

        if (waitForInput) 
        {
            Console.WriteLine("\nНатиснiть будь-яку клавiшу...");
            Console.ReadKey();
        }
    }

    private static async Task AddNewRole()
    {
        Console.Clear();
        Console.WriteLine("--- Додавання нової ролi ---");
        Console.Write("Введiть назву для нової ролi: ");
        string roleName = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(roleName))
        {
            var newRoleDto = new RoleCreateDto { RoleName = roleName };
            var createdRole = await _roleRepository.AddRoleAsync(newRoleDto);
            Console.WriteLine($"\nУСПiХ! Створено нову роль: ID: {createdRole.RoleID}, Назва: {createdRole.RoleName}");
        }
        else { Console.WriteLine("Назва ролi не може бути порожньою."); }
        Console.WriteLine("\nНатиснiть будь-яку клавiшу..."); 
        Console.ReadKey();
    }

    private static async Task UpdateExistingRole()
    {
        Console.Clear();
        Console.WriteLine("--- Оновлення iснуючої ролi ---");
        await ShowAllRoles(waitForInput: false); 
        Console.Write("\nВведiть ID ролi для оновлення: ");
        if (int.TryParse(Console.ReadLine(), out int roleId))
        {
            Console.Write($"Введiть нову назву для ролi з ID {roleId}: ");
            string newName = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(newName))
            {
                await _roleRepository.UpdateRoleAsync(new RoleUpdateDto { RoleID = roleId, RoleName = newName });
                Console.WriteLine("\nУСПiХ! Роль було оновлено.");
            }
            else { Console.WriteLine("Назва не може бути порожньою."); }
        }
        else { Console.WriteLine("Помилка: введено не число."); }
        Console.WriteLine("\nНатиснiть будь-яку клавiшу...");
        Console.ReadKey();
    }

    private static async Task DeleteExistingRole()
    {
        Console.Clear();
        Console.WriteLine("--- Видалення iснуючої ролi ---");
        await ShowAllRoles(waitForInput: false); 
        Console.Write("\nВведiть ID ролi для видалення: ");
        if (int.TryParse(Console.ReadLine(), out int roleId))
        {
            await _roleRepository.DeleteRoleAsync(roleId);
            Console.WriteLine("\nУСПiХ! Роль було видалено.");
        }
        else { Console.WriteLine("Помилка: введено не число."); }
        Console.WriteLine("\nНатиснiть будь-яку клавiшу..."); 
        Console.ReadKey();
    }
    private static async Task RunUsersMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("======= МЕНЮ: Управлiння користувачами =======");
            Console.WriteLine("1. Показати всiх користувачiв");
            Console.WriteLine("2. Додати нового користувача");
            Console.WriteLine("3. Оновити користувача");
            Console.WriteLine("4. Видалити користувача");
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("0. Повернутися до головного меню");
            Console.WriteLine("============================================");
            Console.Write("Ваш вибiр: ");
            string choice = Console.ReadLine() ?? "";
            switch (choice)
            {
                case "1": await ShowAllUsers(waitForInput: true); break; 
                case "2": await AddNewUser(); break;
                case "3": await UpdateExistingUser(); break;
                case "4": await DeleteExistingUser(); break;
                case "0": return;
                default: ShowPlaceholder("Невiрний вибiр. Спробуйте ще раз."); break;
            }
        }
    }

    private static async Task ShowAllUsers(bool waitForInput = false)
    {
        Console.WriteLine("--- Список всiх користувачiв ---");
        var users = await _userRepository.GetAllUsersAsync();
        if (users.Any())
        {
            foreach (var user in users)
            {
                Console.WriteLine($"ID: {user.UserID}, Логiн: {user.Login}, iм'я: {user.FirstName} {user.LastName}, Роль: {user.RoleName}");
            }
        }
        else { Console.WriteLine("Список користувачiв порожнiй."); }

        if (waitForInput) 
        {
            Console.WriteLine("\nНатиснiть будь-яку клавiшу...");
            Console.ReadKey();
        }
    }

    private static async Task AddNewUser()
    {
        Console.Clear();
        Console.WriteLine("--- Додавання нового користувача ---");
        await ShowAllRoles(waitForInput: false); 
        Console.Write("\nВведiть логiн: ");
        string login = Console.ReadLine() ?? "";
        Console.Write("Введiть пароль: ");
        string password = Console.ReadLine() ?? "";
        Console.Write("Введiть Email: ");
        string email = Console.ReadLine() ?? "";
        Console.Write("Введiть iм'я (First Name): ");
        string firstName = Console.ReadLine() ?? "";
        Console.Write("Введiть прiзвище (Last Name): ");
        string lastName = Console.ReadLine() ?? "";
        Console.Write("Введiть ID ролi (з списку вище): ");
        int.TryParse(Console.ReadLine(), out int roleId);

        if (!string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(password) && roleId > 0 && !string.IsNullOrWhiteSpace(email))
        {
            var userToCreate = new UserCreateDto { Login = login, Password = password, Email = email, FirstName = firstName, LastName = lastName, RoleID = roleId };
            var createdUser = await _userRepository.AddUserAsync(userToCreate);
            Console.WriteLine($"\nУСПiХ! Створено користувача: {createdUser.Login} з роллю '{createdUser.RoleName}'");
        }
        else { Console.WriteLine("\nПомилка: Логiн, пароль, Email та ID ролi є обов'язковими."); }
        Console.WriteLine("\nНатиснiть будь-яку клавiшу..."); 
        Console.ReadKey();
    }

    private static async Task UpdateExistingUser()
    {
        Console.Clear();
        Console.WriteLine("--- Оновлення користувача ---");
        await ShowAllUsers(waitForInput: false); 
        Console.Write("\nВведiть ID користувача для оновлення: ");
        if (int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("\nВведiть новi данi:");
            Console.Write("Нове iм'я (First Name): ");
            string firstName = Console.ReadLine() ?? "";
            Console.Write("Нове прiзвище (Last Name): ");
            string lastName = Console.ReadLine() ?? "";
            Console.Write("Новий Email: ");
            string email = Console.ReadLine() ?? "";
            await ShowAllRoles(waitForInput: false); 
            Console.Write("Новий ID ролi: ");
            int.TryParse(Console.ReadLine(), out int roleId);

            if (!string.IsNullOrWhiteSpace(email) && roleId > 0)
            {
                await _userRepository.UpdateUserAsync(new UserUpdateDto { UserID = userId, FirstName = firstName, LastName = lastName, Email = email, RoleID = roleId });
                Console.WriteLine("\nУСПiХ! Данi користувача було оновлено.");
            }
            else { Console.WriteLine("\nПомилка: Email та ID ролi є обов'язковими."); }
        }
        else { Console.WriteLine("Помилка: введено не число."); }
        Console.WriteLine("\nНатиснiть будь-яку клавiшу..."); 
        Console.ReadKey();
    }

    private static async Task DeleteExistingUser()
    {
        Console.Clear();
        Console.WriteLine("--- Видалення користувача ---");
        await ShowAllUsers(waitForInput: false); 
        Console.Write("\nВведiть ID користувача для видалення: ");
        if (int.TryParse(Console.ReadLine(), out int userId))
        {
            await _userRepository.DeleteUserAsync(userId);
            Console.WriteLine("\nУСПiХ! Користувача було видалено.");
        }
        else { Console.WriteLine("Помилка: введено не число."); }
        Console.WriteLine("\nНатиснiть будь-яку клавiшу..."); 
        Console.ReadKey();
    }

    private static void ShowPlaceholder(string message)
    {
        Console.Clear();
        Console.WriteLine(message);
        Console.WriteLine("\nНатиснiть будь-яку клавiшу...");
        Console.ReadKey();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                services.AddDbContext<AdminControlContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                });
                services.AddScoped<IRoleRepository, RoleRepository>();
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IUserBankCardRepository, UserBankCardRepository>();
            })
            .ConfigureAppConfiguration((hostContext, configBuilder) =>
            {
                configBuilder.SetBasePath(Directory.GetCurrentDirectory());
                configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureLogging(logging =>
            {
                //logging.ClearProviders();
            });
}