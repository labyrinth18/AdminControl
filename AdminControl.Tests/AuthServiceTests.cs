using AdminControl.BLL.Services;
using AdminControl.DALEF.Concrete;
using AdminControl.DALEF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace AdminControl.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        private DbContextOptions<AdminControlContext> CreateNewDbOptions()
        {
            return new DbContextOptionsBuilder<AdminControlContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [TestMethod]
        public async Task AuthenticateAsync_CorrectCredentials_ReturnsUser()
        {
            var options = CreateNewDbOptions();

            using (var context = new AdminControlContext(options))
            {
                var passwordHash = "mypassword".GetHashCode().ToString();

ї                var role = new Role { RoleName = "Admin" };

                context.Users.Add(new User
                {
                    Login = "test_user",
                    PasswordHash = passwordHash,
                    Role = role,
                    Email = "test@mail.com", 
                    FirstName = "Test",
                    LastName = "User",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                context.SaveChanges();
            }


            using (var context = new AdminControlContext(options))
            {

                var repository = new UserRepository(context);
                var authService = new AuthService(repository);

                
                var result = await authService.AuthenticateAsync("test_user", "mypassword");

                
                Assert.IsNotNull(result);
                Assert.AreEqual("test_user", result.Login);
                Assert.AreEqual("Admin", result.RoleName);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))] 
        public async Task AuthenticateAsync_WrongPassword_ThrowsException()
        {
            
            var options = CreateNewDbOptions();
            using (var context = new AdminControlContext(options))
            {
                var passwordHash = "correct_password".GetHashCode().ToString();
                var role = new Role { RoleName = "User" };

                context.Users.Add(new User
                {
                    Login = "test_user",
                    PasswordHash = passwordHash,
                    Role = role,
                    Email = "test@mail.com",
                    FirstName = "Test",
                    LastName = "User",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                context.SaveChanges();
            }

            
            using (var context = new AdminControlContext(options))
            {
                var repository = new UserRepository(context);
                var authService = new AuthService(repository);


                await authService.AuthenticateAsync("test_user", "WRONG_PASSWORD");
            }
            
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task AuthenticateAsync_UserNotFound_ThrowsException()
        {
            var options = CreateNewDbOptions();

            using (var context = new AdminControlContext(options))
            {
                var repository = new UserRepository(context);
                var authService = new AuthService(repository);

                await authService.AuthenticateAsync("ghost_user", "any_password");
            }
        }
    }
}