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
            var mockAuthService = new Mock<IAuthService>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var viewModel = new LoginViewModel(mockAuthService.Object, mockServiceProvider.Object);

            viewModel.Login = "";

            Assert.IsTrue(viewModel.HasErrors);
            Assert.IsFalse(string.IsNullOrEmpty(viewModel["Login"]));
        }

      

        [TestMethod]
        public void LoginViewModel_ValidLogin_ShouldNotHaveErrors()
        {
            var mockAuthService = new Mock<IAuthService>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var viewModel = new LoginViewModel(mockAuthService.Object, mockServiceProvider.Object);

            viewModel.Login = "admin";

            Assert.IsFalse(viewModel.HasErrors);
            Assert.AreEqual(string.Empty, viewModel["Login"]);
        }

        [TestMethod]
        public void LoginViewModel_IDataErrorInfo_Error_ShouldBeEmpty()
        {
            var mockAuthService = new Mock<IAuthService>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var viewModel = new LoginViewModel(mockAuthService.Object, mockServiceProvider.Object);

            Assert.AreEqual(string.Empty, viewModel.Error);
        }

        #endregion

        #region AddEditUserViewModel Tests

        [TestMethod]
        public void AddEditUserViewModel_EmptyLogin_ShouldHaveErrors()
        {
            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>());

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            viewModel.SetMode(isEditMode: false);

            viewModel.Login = "";

            var error = viewModel.ValidateAndGetError();
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("Логін"));
        }

        [TestMethod]
        public void AddEditUserViewModel_ValidLogin_ShouldNotHaveLoginError()
        {
            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>
            {
                new RoleDto { RoleID = 1, RoleName = "Admin" }
            });

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            viewModel.SetMode(isEditMode: false);

            viewModel.Login = "valid_user";
            viewModel.Password = "Password123";
            viewModel.ConfirmPassword = "Password123";
            viewModel.FirstName = "Іван";
            viewModel.LastName = "Петренко";
            viewModel.Email = "test@test.com";

            System.Threading.Thread.Sleep(100);
            
            var error = viewModel.ValidateAndGetError();
            if (error != null)
            {
                Assert.IsFalse(error.Contains("Логін"));
            }
        }

        [TestMethod]
        public void AddEditUserViewModel_InvalidLoginWithSpecialChars_ShouldHaveErrors()
        {
            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>());

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            viewModel.SetMode(isEditMode: false);

            viewModel.Login = "user@test!"; 
            var error = viewModel.ValidateAndGetError();
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("латинські") || error.Contains("Логін"));
        }

        [TestMethod]
        public void AddEditUserViewModel_InvalidEmail_ShouldHaveErrors()
        {

            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>());

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            viewModel.SetMode(isEditMode: false);


            viewModel.Login = "validuser";
            viewModel.Password = "Password123";
            viewModel.ConfirmPassword = "Password123";
            viewModel.FirstName = "Іван";
            viewModel.LastName = "Петренко";
            viewModel.Email = "invalid-email"; 



            var error = viewModel.ValidateAndGetError();
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("email") || error.Contains("Email"));
        }

        [TestMethod]
        public void AddEditUserViewModel_ValidEmail_ShouldNotHaveEmailError()
        {


            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>
            {
                new RoleDto { RoleID = 1, RoleName = "Admin" }
            });

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            viewModel.SetMode(isEditMode: false);


            viewModel.Login = "validuser";
            viewModel.Password = "Password123";
            viewModel.ConfirmPassword = "Password123";
            viewModel.FirstName = "Іван";
            viewModel.LastName = "Петренко";
            viewModel.Email = "valid@email.com";

            System.Threading.Thread.Sleep(100);


            var error = viewModel.ValidateAndGetError();
            if (error != null)
            {
                Assert.IsFalse(error.ToLower().Contains("email"));
            }
        }

        [TestMethod]
        public void AddEditUserViewModel_ShortFirstName_ShouldHaveErrors()
        {

            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>());

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            viewModel.SetMode(isEditMode: false);


            viewModel.Login = "validuser";
            viewModel.Password = "Password123";
            viewModel.ConfirmPassword = "Password123";
            viewModel.FirstName = "A"; // Менше 2 символів


            var error = viewModel.ValidateAndGetError();
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("Ім'я") || error.Contains("мінімум 2"));
        }

        [TestMethod]
        public void AddEditUserViewModel_EmptyPassword_InCreateMode_ShouldHaveErrors()
        {

            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>());

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            viewModel.SetMode(isEditMode: false); 



            viewModel.Login = "validuser";
            viewModel.Password = ""; 

            var error = viewModel.ValidateAndGetError();
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("Пароль") || error.Contains("пароль"));
        }

        [TestMethod]
        public void AddEditUserViewModel_WeakPassword_ShouldHaveErrors()
        {
            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>());

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            viewModel.SetMode(isEditMode: false);

            viewModel.Login = "validuser";
            viewModel.Password = "password123"; 

            // Assert
            var error = viewModel.ValidateAndGetError();
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("велику літеру") || error.Contains("Пароль"));
        }

        [TestMethod]
        public void AddEditUserViewModel_PasswordsMismatch_ShouldHaveErrors()
        {
           
            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>());

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            viewModel.SetMode(isEditMode: false);

           
            viewModel.Login = "validuser";
            viewModel.Password = "Password123";
            viewModel.ConfirmPassword = "DifferentPassword123";

            
            var error = viewModel.ValidateAndGetError();
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("співпадають") || error.Contains("Паролі"));
        }

        [TestMethod]
        public void AddEditUserViewModel_EditMode_ShouldNotRequirePassword()
        {
         
            var mockUserRepo = new Mock<IUserRepository>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            mockRoleRepo.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<RoleDto>
            {
                new RoleDto { RoleID = 1, RoleName = "Admin" }
            });

            var viewModel = new AddEditUserViewModel(mockUserRepo.Object, mockRoleRepo.Object);
            
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


            var error = viewModel.ValidateAndGetError();
            if (error != null)
            {
                Assert.IsFalse(error.Contains("Пароль"));
            }
        }

        #endregion
    }
}
//[TestMethod]
//public void LoginViewModel_ShortLogin_ShouldHaveErrors()
//{
//    // Arrange
//    var mockAuthService = new Mock<IAuthService>();
//    var mockServiceProvider = new Mock<IServiceProvider>();
//    var viewModel = new LoginViewModel(mockAuthService.Object, mockServiceProvider.Object);

//    // Act
//    viewModel.Login = "ab"; // Менше 3 символів

//    // Assert - перевіряємо через IDataErrorInfo індексатор
//    Assert.IsTrue(viewModel.HasErrors);
//    Assert.IsTrue(viewModel["Login"].Contains("мінімум 3"));
//}