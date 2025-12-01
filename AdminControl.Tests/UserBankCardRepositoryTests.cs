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
            int targetUserId;

            using (var context = new AdminControlContext(options))
            {
                
                var role = new Role
                {
                    RoleName = "Tester",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                context.Roles.Add(role);

                var user = new User
                {
                    Login = "card_owner",
                    Email = "owner@test.com",
                    PasswordHash = "hash",
                    FirstName = "Ivan",
                    LastName = "Testenko",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Role = role
                };
                context.Users.Add(user);

                context.SaveChanges();
                targetUserId = user.UserID; 

                
                context.UserBankCards.Add(new UserBankCard
                {
                    User = user, 
                    UserID = targetUserId,
                    CardHolderName = "IVAN TESTENKO",
                    CardNumberSuffix = "1111",
                    EncryptedCardData = "encrypted_data_1",
                    CreatedAt = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddYears(1)
                });

                context.UserBankCards.Add(new UserBankCard
                {
                    User = user,
                    UserID = targetUserId,
                    CardHolderName = "IVAN TESTENKO",
                    CardNumberSuffix = "2222",
                    EncryptedCardData = "encrypted_data_2",
                    CreatedAt = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddYears(2)
                });

                context.SaveChanges();
            }

            
            using (var context = new AdminControlContext(options))
            {
                var repository = new UserBankCardRepository(context);

                var result = await repository.GetCardsByUserIdAsync(targetUserId);

                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Count()); 

                Assert.IsTrue(result.Any(c => c.CardNumberSuffix == "1111"));
                Assert.IsTrue(result.Any(c => c.CardNumberSuffix == "2222"));
            }
        }
    }
}