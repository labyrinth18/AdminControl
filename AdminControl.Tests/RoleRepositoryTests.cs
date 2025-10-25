using AdminControl.DALEF.Concrete;
using AdminControl.DALEF.Models;
using AdminControl.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminControl.Tests
{
    [TestClass]
    public class RoleRepositoryTests
    {
        private DbContextOptions<AdminControlContext> CreateNewDbOptions()
        {
            return new DbContextOptionsBuilder<AdminControlContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
        }

        [TestMethod]
        public async Task GetAllRolesAsync_WhenDbIsEmpty_ShouldReturnEmptyCollection()
        {
            var options = CreateNewDbOptions();

            using (var context = new AdminControlContext(options))
            {
                var repository = new RoleRepository(context);

                var result = await repository.GetAllRolesAsync();

                Assert.IsNotNull(result);
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public async Task GetAllRolesAsync_WhenDbHasData_ShouldReturnAllRoles()
        {
            var options = CreateNewDbOptions();

            using (var context = new AdminControlContext(options))
            {
                context.Roles.Add(new Role { RoleID = 1, RoleName = "Admin", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
                context.Roles.Add(new Role { RoleID = 2, RoleName = "User", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
                context.SaveChanges();
            }

            using (var context = new AdminControlContext(options))
            {
                var repository = new RoleRepository(context);
                var result = await repository.GetAllRolesAsync();

                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Count());
                Assert.AreEqual("Admin", result.First().RoleName);
            }
        }

        [TestMethod]
        public async Task AddRoleAsync_ShouldAddRoleToDatabase_AndReturnDtoWithId()
        {
            var options = CreateNewDbOptions();
            var newRoleDto = new RoleCreateDto { RoleName = "Test Role" };
            RoleDto createdRoleDto;

            using (var context = new AdminControlContext(options))
            {
                var repository = new RoleRepository(context);
                createdRoleDto = await repository.AddRoleAsync(newRoleDto);
            }

            Assert.IsNotNull(createdRoleDto);
            Assert.IsTrue(createdRoleDto.RoleID > 0);
            Assert.AreEqual("Test Role", createdRoleDto.RoleName);

            using (var context = new AdminControlContext(options))
            {
                Assert.AreEqual(1, context.Roles.Count());
                var roleInDb = await context.Roles.FirstAsync();
                Assert.AreEqual("Test Role", roleInDb.RoleName);
            }
        }

        [TestMethod]
        public async Task UpdateRoleAsync_WhenRoleExists_ShouldUpdateRoleName()
        {
            var options = CreateNewDbOptions();
            var originalRole = new Role { RoleID = 1, RoleName = "Old Name", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

            using (var context = new AdminControlContext(options))
            {
                context.Roles.Add(originalRole);
                context.SaveChanges();
            }

            var roleToUpdateDto = new RoleUpdateDto { RoleID = 1, RoleName = "New Name" };

            using (var context = new AdminControlContext(options))
            {
                var repository = new RoleRepository(context);
                await repository.UpdateRoleAsync(roleToUpdateDto);
            }

            using (var context = new AdminControlContext(options))
            {
                var updatedRoleInDb = await context.Roles.FindAsync(1);

                Assert.IsNotNull(updatedRoleInDb);
                Assert.AreEqual("New Name", updatedRoleInDb.RoleName);
                Assert.AreNotEqual(originalRole.UpdatedAt, updatedRoleInDb.UpdatedAt);
            }
        }

        [TestMethod]
        public async Task DeleteRoleAsync_WhenRoleExists_ShouldRemoveRoleFromDatabase()
        {
            var options = CreateNewDbOptions();
            var roleToDelete = new Role { RoleID = 1, RoleName = "To Be Deleted", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

            using (var context = new AdminControlContext(options))
            {
                context.Roles.Add(roleToDelete);
                context.SaveChanges();
            }

            using (var context = new AdminControlContext(options))
            {
                var repository = new RoleRepository(context);
                await repository.DeleteRoleAsync(1);
            }

            using (var context = new AdminControlContext(options))
            {
                Assert.AreEqual(0, context.Roles.Count());

                var deletedRole = await context.Roles.FindAsync(1);
                Assert.IsNull(deletedRole);
            }
        }
    }
}