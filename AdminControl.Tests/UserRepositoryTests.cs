using AdminControl.DALEF.Concrete;
using AdminControl.DALEF.Models;
using AdminControl.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AdminControl.Tests
{
    [TestClass]
    public class UserRepositoryTests
    {
        private DbContextOptions<AdminControlContext> CreateNewDbOptions()
        {
            return new DbContextOptionsBuilder<AdminControlContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
        }

        [TestMethod]
        public async Task GetAllUsersAsync_WhenDbIsEmpty_ShouldReturnEmptyCollection()
        {
            var options = CreateNewDbOptions();
            using (var context = new AdminControlContext(options))
            {
                var repository = new UserRepository(context);
                var result = await repository.GetAllUsersAsync();
                Assert.IsNotNull(result);
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public async Task GetAllUsersAsync_WhenDbHasData_ShouldReturnUsersWithRoles()
        {
            var options = CreateNewDbOptions();
            using (var context = new AdminControlContext(options))
            {
                var role = new Role { RoleID = 1, RoleName = "Admin", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
                context.Roles.Add(role);
                context.Users.Add(new User
                {
                    UserID = 1,
                    Login = "test_user",
                    Email = "test@test.com",
                    PasswordHash = "hash",
                    FirstName = "Test",
                    LastName = "User",
                    Role = role,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                context.SaveChanges();
            }

            using (var context = new AdminControlContext(options))
            {
                var repository = new UserRepository(context);
                var result = await repository.GetAllUsersAsync();
                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Count());
                var userDto = result.First();
                Assert.AreEqual("test_user", userDto.Login);
                Assert.AreEqual("Admin", userDto.RoleName);
            }
        }

        [TestMethod]
        public async Task AddUserAsync_ShouldAddUserAndReturnDtoWithRoleName()
        {
            var options = CreateNewDbOptions();
            using (var context = new AdminControlContext(options))
            {
                context.Roles.Add(new Role { RoleID = 1, RoleName = "Tester", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
                context.SaveChanges();
            }

            var userToCreate = new UserCreateDto
            {
                Login = "new_user",
                Password = "password123",
                Email = "new@user.com",
                FirstName = "Test",
                LastName = "User",
                RoleID = 1
            };
            UserDto createdUserDto;

            using (var context = new AdminControlContext(options))
            {
                var repository = new UserRepository(context);
                createdUserDto = await repository.AddUserAsync(userToCreate);
            }

            Assert.IsNotNull(createdUserDto);
            Assert.IsTrue(createdUserDto.UserID > 0);
            Assert.AreEqual("Tester", createdUserDto.RoleName);

            using (var context = new AdminControlContext(options))
            {
                var userInDb = await context.Users.Include(u => u.Role).FirstAsync();
                Assert.AreEqual("new_user", userInDb.Login);
                Assert.AreEqual("Tester", userInDb.Role.RoleName);
            }
        }

        [TestMethod]
        public async Task UpdateUserAsync_WhenUserExists_ShouldUpdateDetailsAndRole()
        {
            var options = CreateNewDbOptions();
            using (var context = new AdminControlContext(options))
            {
                var oldRole = new Role { RoleID = 1, RoleName = "Old Role", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
                var newRole = new Role { RoleID = 2, RoleName = "New Role", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
                context.Roles.AddRange(oldRole, newRole);
                context.Users.Add(new User
                {
                    UserID = 1,
                    Login = "user_to_update",
                    FirstName = "Initial",
                    LastName = "Name",
                    Email = "initial@test.com",
                    PasswordHash = "hash",
                    Role = oldRole,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                context.SaveChanges();
            }

            var userToUpdate = new UserUpdateDto
            {
                UserID = 1,
                FirstName = "Updated",
                LastName = "Name",
                Email = "updated@test.com",
                RoleID = 2
            };

            using (var context = new AdminControlContext(options))
            {
                var repository = new UserRepository(context);
                await repository.UpdateUserAsync(userToUpdate);
            }

            using (var context = new AdminControlContext(options))
            {
                var userInDb = await context.Users.Include(u => u.Role).FirstAsync(u => u.UserID == 1);
                Assert.AreEqual("Updated", userInDb.FirstName);
                Assert.AreEqual("New Role", userInDb.Role.RoleName);
            }
        }

        [TestMethod]
        public async Task DeleteUserAsync_WhenUserExists_ShouldRemoveUserFromDatabase()
        {
            var options = CreateNewDbOptions();
            using (var context = new AdminControlContext(options))
            {
                var role = new Role { RoleID = 1, RoleName = "SomeRole", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
                context.Roles.Add(role);
                context.Users.Add(new User
                {
                    UserID = 1,
                    Login = "user_to_delete",
                    PasswordHash = "hash",
                    Email = "delete@test.com",
                    FirstName = "Test",
                    LastName = "Delete",
                    Role = role,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                context.SaveChanges();
            }

            using (var context = new AdminControlContext(options))
            {
                var repository = new UserRepository(context);
                await repository.DeleteUserAsync(1);
            }

            using (var context = new AdminControlContext(options))
            {
                var userCount = await context.Users.CountAsync();
                Assert.AreEqual(0, userCount);
            }
        }
    }
}