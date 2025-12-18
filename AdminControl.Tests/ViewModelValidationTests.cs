using AdminControl.BLL.Interfaces;
using AdminControl.DAL;
using AdminControl.DTO;
using AdminControl.WPF.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminControl.Tests
{
    [TestClass]
    public class ViewModelValidationTests
    {
        #region LoginViewModel Tests (IDataErrorInfo)

        [TestMethod]
        public void LoginViewModel_EmptyLogin_ShouldHaveErrors()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var viewModel = new LoginViewModel(mockAuthService.Object, mockServiceProvider.Object);

            // Act
            viewModel.Login = "";

            // Assert - перевіряємо через IDataErrorInfo
            Assert.IsTrue(viewModel.HasErrors);
            Assert.IsFalse(string.IsNullOrEmpty(viewModel["Login"]));
        }

        [TestMethod]
        public void LoginViewModel_ShortLogin_ShouldHaveErrors()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var viewModel = new LoginViewModel(mockAuthService.Object, mockServiceProvider.Object);

            // Act
            viewModel.Login = "ab"; // Менше 3 символів

            // Assert - перевіряємо через IDataErrorInfo індексатор
            Assert.IsTrue(viewModel.HasErrors);
            Assert.IsTrue(viewModel["Login"].Contains("мінімум 3"));
        }

        [TestMethod]
        public void LoginViewModel_ValidLogin_ShouldNotHaveErrors()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var viewModel = new LoginViewModel(mockAuthService.Object, mockServiceProvider.Object);

            // Act
            viewModel.Login = "admin";

            // Assert - IDataErrorInfo повертає порожній рядок коли немає помилок
            Assert.IsFalse(viewModel.HasErrors);
            Assert.AreEqual(string.Empty, viewModel["Login"]);
        }

        [TestMethod]
        public void LoginViewModel_IDataErrorInfo_Error_ShouldBeEmpty()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var viewModel = new LoginViewModel(mockAuthService.Object, mockServiceProvider.Object);

            // Assert - властивість Error має бути порожньою (за конвенцією IDataErrorInfo)
            Assert.AreEqual(string.Empty, viewModel.Error);
        }

        #endregion

        #region AddEditUserViewModel Tests

        [TestMethod]
        public void AddEditUserViewModel_EmptyLogin_ShouldHaveErrors()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>());

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            viewModel.SetMode(isEditMode: false);

            // Act
            viewModel.Login = "";

            // Assert - використовуємо метод валідації
            var error = viewModel.ValidateAndGetError();
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("Логін"));
        }

        [TestMethod]
        public void AddEditUserViewModel_ValidLogin_ShouldNotHaveLoginError()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>
            {
                new RoleDto { RoleID = 1, RoleName = "Admin" }
            });

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            viewModel.SetMode(isEditMode: false);

            // Act - заповнюємо всі обов'язкові поля
            viewModel.Login = "valid_user";
            viewModel.Password = "Password123";
            viewModel.ConfirmPassword = "Password123";
            viewModel.FirstName = "Іван";
            viewModel.LastName = "Петренко";
            viewModel.Email = "test@test.com";

            // Чекаємо завантаження ролей
            System.Threading.Thread.Sleep(100);
            
            // Assert
            var error = viewModel.ValidateAndGetError();
            // Може бути помилка через роль, але не через логін
            if (error != null)
            {
                Assert.IsFalse(error.Contains("Логін"));
            }
        }

        [TestMethod]
        public void AddEditUserViewModel_InvalidLoginWithSpecialChars_ShouldHaveErrors()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>());

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            viewModel.SetMode(isEditMode: false);

            // Act
            viewModel.Login = "user@test!"; // Спецсимволи

            // Assert
            var error = viewModel.ValidateAndGetError();
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("латинські") || error.Contains("Логін"));
        }

        [TestMethod]
        public void AddEditUserViewModel_InvalidEmail_ShouldHaveErrors()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>());

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            viewModel.SetMode(isEditMode: false);

            // Act - заповнюємо поля до Email
            viewModel.Login = "validuser";
            viewModel.Password = "Password123";
            viewModel.ConfirmPassword = "Password123";
            viewModel.FirstName = "Іван";
            viewModel.LastName = "Петренко";
            viewModel.Email = "invalid-email"; // Невірний email

            // Assert
            var error = viewModel.ValidateAndGetError();
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("email") || error.Contains("Email"));
        }

        [TestMethod]
        public void AddEditUserViewModel_ValidEmail_ShouldNotHaveEmailError()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>
            {
                new RoleDto { RoleID = 1, RoleName = "Admin" }
            });

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            viewModel.SetMode(isEditMode: false);

            // Act
            viewModel.Login = "validuser";
            viewModel.Password = "Password123";
            viewModel.ConfirmPassword = "Password123";
            viewModel.FirstName = "Іван";
            viewModel.LastName = "Петренко";
            viewModel.Email = "valid@email.com";

            System.Threading.Thread.Sleep(100);

            // Assert
            var error = viewModel.ValidateAndGetError();
            if (error != null)
            {
                Assert.IsFalse(error.ToLower().Contains("email"));
            }
        }

        [TestMethod]
        public void AddEditUserViewModel_ShortFirstName_ShouldHaveErrors()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>());

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            viewModel.SetMode(isEditMode: false);

            // Act
            viewModel.Login = "validuser";
            viewModel.Password = "Password123";
            viewModel.ConfirmPassword = "Password123";
            viewModel.FirstName = "A"; // Менше 2 символів

            // Assert
            var error = viewModel.ValidateAndGetError();
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("Ім'я") || error.Contains("мінімум 2"));
        }

        [TestMethod]
        public void AddEditUserViewModel_EmptyPassword_InCreateMode_ShouldHaveErrors()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>());

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            viewModel.SetMode(isEditMode: false); // Режим створення

            // Act
            viewModel.Login = "validuser";
            viewModel.Password = ""; // Порожній пароль

            // Assert
            var error = viewModel.ValidateAndGetError();
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("Пароль") || error.Contains("пароль"));
        }

        [TestMethod]
        public void AddEditUserViewModel_WeakPassword_ShouldHaveErrors()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>());

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            viewModel.SetMode(isEditMode: false);

            // Act - пароль без великої літери
            viewModel.Login = "validuser";
            viewModel.Password = "password123"; // Без великої літери

            // Assert
            var error = viewModel.ValidateAndGetError();
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("велику літеру") || error.Contains("Пароль"));
        }

        [TestMethod]
        public void AddEditUserViewModel_PasswordsMismatch_ShouldHaveErrors()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>());

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            viewModel.SetMode(isEditMode: false);

            // Act
            viewModel.Login = "validuser";
            viewModel.Password = "Password123";
            viewModel.ConfirmPassword = "DifferentPassword123"; // Не співпадає

            // Assert
            var error = viewModel.ValidateAndGetError();
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("співпадають") || error.Contains("Паролі"));
        }

        [TestMethod]
        public void AddEditUserViewModel_EditMode_ShouldNotRequirePassword()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>
            {
                new RoleDto { RoleID = 1, RoleName = "Admin" }
            });

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            
            // Встановлюємо режим редагування з існуючим користувачем
            var existingUser = new UserDto
            {
                UserID = 1,
                Login = "existinguser",
                FirstName = "Іван",
                LastName = "Петренко",
                Email = "test@test.com",
                RoleID = 1,
                RoleName = "Admin",
                IsActive = true
            };

            System.Threading.Thread.Sleep(100);
            viewModel.SetMode(isEditMode: true, existingUser);

            // Act - не вводимо пароль (в режимі редагування це нормально)

            // Assert
            var error = viewModel.ValidateAndGetError();
            // В режимі редагування пароль не обов'язковий
            if (error != null)
            {
                Assert.IsFalse(error.Contains("Пароль"));
            }
        }

        #endregion
    }
}
