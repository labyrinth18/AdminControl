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
    public class UserBankCardRepositoryTests
    {
        private DbContextOptions<AdminControlContext> CreateNewDbOptions()
        {
            return new DbContextOptionsBuilder<AdminControlContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [TestMethod]
        public async Task GetCardsByUserIdAsync_WhenUserHasCards_ShouldReturnCards()
        {
            var options = CreateNewDbOptions();
            using (var context = new AdminControlContext(options))
            {
                var user = new User { UserID = 1, Login = "test", Email = "e", PasswordHash = "h", FirstName = "f", LastName = "l", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, RoleID = 1 }; // Потрібна роль, хоча б фіктивна
                var role = new Role { RoleID = 1, RoleName = "r", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
                context.Roles.Add(role);
                context.Users.Add(user);
                context.UserBankCards.Add(new UserBankCard { BankCardID = 1, User = user, CardHolderName = "H", CardNumberSuffix = "1234", EncryptedCardData = "e", CreatedAt = DateTime.UtcNow, ExpiryDate = DateTime.UtcNow });
                context.UserBankCards.Add(new UserBankCard { BankCardID = 2, User = user, CardHolderName = "H2", CardNumberSuffix = "5678", EncryptedCardData = "e2", CreatedAt = DateTime.UtcNow, ExpiryDate = DateTime.UtcNow });
                context.SaveChanges();
            }

            using (var context = new AdminControlContext(options))
            {
                var repository = new UserBankCardRepository(context);
                var result = await repository.GetCardsByUserIdAsync(1);

                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Count());
                Assert.AreEqual("1234", result.First().CardNumberSuffix);
            }
        }
    }
}