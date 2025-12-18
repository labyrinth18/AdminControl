using AdminControl.DAL;
using AdminControl.DTO;
using AdminControl.WPF.Infrastructure;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace AdminControl.WPF.ViewModels
{
    public class AddEditUserViewModel : ViewModelBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        private bool _isEditMode;
        private int _userId;
        private string _login = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _email = string.Empty;
        private string _phoneNumber = string.Empty;
        private string _address = string.Empty;
        private string _selectedGender = string.Empty;
        private RoleDto? _selectedRole;
        private bool _isActive = true;
        private string _errorMessage = string.Empty;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ObservableCollection<RoleDto> Roles { get; set; }
        public ObservableCollection<string> Genders { get; set; }

        public string WindowTitle => _isEditMode ? "Редагування користувача" : "Додавання користувача";
        public string SaveButtonText => _isEditMode ? "Зберегти" : "Додати";
        public Visibility PasswordVisibility => _isEditMode ? Visibility.Collapsed : Visibility.Visible;
        public bool IsLoginEditable => !_isEditMode;

        #region Properties

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
            }
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        public string SelectedGender
        {
            get => _selectedGender;
            set
            {
                _selectedGender = value;
                OnPropertyChanged();
            }
        }

        public RoleDto? SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged();
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public AddEditUserViewModel(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            
            Roles = new ObservableCollection<RoleDto>();
            Genders = new ObservableCollection<string> { "Чоловік", "Жінка", "Інше" };

            SaveCommand = new RelayCommand(ExecuteSave);
            CancelCommand = new RelayCommand(ExecuteCancel);

            LoadRoles();
        }

        public void SetMode(bool isEditMode, UserDto? userToEdit = null)
        {
            _isEditMode = isEditMode;
            ErrorMessage = string.Empty;

            if (isEditMode && userToEdit != null)
            {
                _userId = userToEdit.UserID;
                Login = userToEdit.Login;
                FirstName = userToEdit.FirstName;
                LastName = userToEdit.LastName;
                Email = userToEdit.Email;
                PhoneNumber = userToEdit.PhoneNumber ?? string.Empty;
                Address = userToEdit.Address ?? string.Empty;
                SelectedGender = userToEdit.Gender ?? string.Empty;
                IsActive = userToEdit.IsActive;

                foreach (var role in Roles)
                {
                    if (role.RoleID == userToEdit.RoleID)
                    {
                        SelectedRole = role;
                        break;
                    }
                }
            }
            else
            {
                _userId = 0;
                Login = string.Empty;
                Password = string.Empty;
                ConfirmPassword = string.Empty;
                FirstName = string.Empty;
                LastName = string.Empty;
                Email = string.Empty;
                PhoneNumber = string.Empty;
                Address = string.Empty;
                SelectedGender = string.Empty;
                IsActive = true;
                SelectedRole = Roles.Count > 0 ? Roles[0] : null;
            }

            OnPropertyChanged(nameof(WindowTitle));
            OnPropertyChanged(nameof(SaveButtonText));
            OnPropertyChanged(nameof(PasswordVisibility));
            OnPropertyChanged(nameof(IsLoginEditable));
        }

        private async void LoadRoles()
        {
            try
            {
                var roles = await _roleRepository.GetAllRolesAsync();
                Roles.Clear();
                foreach (var role in roles)
                {
                    Roles.Add(role);
                }

                if (Roles.Count > 0 && SelectedRole == null)
                {
                    SelectedRole = Roles[0];
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Помилка завантаження ролей: " + ex.Message;
            }
        }

        #region Валідація

        /// <summary>
        /// Публічний метод для тестування валідації
        /// </summary>
        public string? ValidateAndGetError()
        {
            // Валідація логіну
            if (string.IsNullOrWhiteSpace(Login))
                return "Логін не може бути порожнім";
            if (Login.Length < 3)
                return "Логін повинен містити мінімум 3 символи";
            if (Login.Length > 50)
                return "Логін не може перевищувати 50 символів";
            if (!Regex.IsMatch(Login, @"^[a-zA-Z0-9_]+$"))
                return "Логін може містити лише латинські літери, цифри та _";

            // Валідація паролю (тільки для нових користувачів)
            if (!_isEditMode)
            {
                if (string.IsNullOrWhiteSpace(Password))
                    return "Пароль не може бути порожнім";
                if (Password.Length < 6)
                    return "Пароль повинен містити мінімум 6 символів";
                if (!Regex.IsMatch(Password, @"[A-Z]"))
                    return "Пароль повинен містити хоча б одну велику літеру";
                if (!Regex.IsMatch(Password, @"[a-z]"))
                    return "Пароль повинен містити хоча б одну малу літеру";
                if (!Regex.IsMatch(Password, @"[0-9]"))
                    return "Пароль повинен містити хоча б одну цифру";
                
                if (string.IsNullOrWhiteSpace(ConfirmPassword))
                    return "Підтвердіть пароль";
                if (Password != ConfirmPassword)
                    return "Паролі не співпадають";
            }

            // Валідація імені
            if (string.IsNullOrWhiteSpace(FirstName))
                return "Ім'я не може бути порожнім";
            if (FirstName.Length < 2)
                return "Ім'я повинно містити мінімум 2 символи";

            // Валідація прізвища
            if (string.IsNullOrWhiteSpace(LastName))
                return "Прізвище не може бути порожнім";
            if (LastName.Length < 2)
                return "Прізвище повинно містити мінімум 2 символи";

            // Валідація email
            if (string.IsNullOrWhiteSpace(Email))
                return "Email не може бути порожнім";
            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return "Невірний формат email";

            // Валідація телефону (якщо вказано)
            if (!string.IsNullOrWhiteSpace(PhoneNumber))
            {
                if (!Regex.IsMatch(PhoneNumber, @"^[\+]?[0-9\s\-\(\)]+$"))
                    return "Невірний формат номера телефону";
            }

            // Валідація ролі
            if (SelectedRole == null)
                return "Будь ласка, виберіть роль";

            return null; // Немає помилок
        }

        #endregion

        private async void ExecuteSave(object? parameter)
        {
            var error = ValidateAndGetError();
            if (error != null)
            {
                ErrorMessage = error;
                return;
            }

            try
            {
                if (!_isEditMode)
                {
                    var loginExists = await _userRepository.IsLoginExistsAsync(Login);
                    if (loginExists)
                    {
                        ErrorMessage = "Користувач з таким логіном вже існує";
                        return;
                    }
                }

                var emailExists = await _userRepository.IsEmailExistsAsync(Email, _isEditMode ? _userId : null);
                if (emailExists)
                {
                    ErrorMessage = "Користувач з таким email вже існує";
                    return;
                }

                if (_isEditMode)
                {
                    var updateDto = new UserUpdateDto
                    {
                        UserID = _userId,
                        FirstName = FirstName,
                        LastName = LastName,
                        Email = Email,
                        PhoneNumber = string.IsNullOrWhiteSpace(PhoneNumber) ? null : PhoneNumber,
                        Address = string.IsNullOrWhiteSpace(Address) ? null : Address,
                        Gender = string.IsNullOrWhiteSpace(SelectedGender) ? null : SelectedGender,
                        RoleID = SelectedRole!.RoleID,
                        IsActive = IsActive
                    };

                    await _userRepository.UpdateUserAsync(updateDto);
                    MessageBox.Show("Користувача успішно оновлено!", "Успіх",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var createDto = new UserCreateDto
                    {
                        Login = Login,
                        Password = Password,
                        FirstName = FirstName,
                        LastName = LastName,
                        Email = Email,
                        PhoneNumber = string.IsNullOrWhiteSpace(PhoneNumber) ? null : PhoneNumber,
                        Address = string.IsNullOrWhiteSpace(Address) ? null : Address,
                        Gender = string.IsNullOrWhiteSpace(SelectedGender) ? null : SelectedGender,
                        RoleID = SelectedRole!.RoleID,
                        IsActive = IsActive
                    };

                    await _userRepository.AddUserAsync(createDto);
                    MessageBox.Show("Користувача успішно додано!", "Успіх",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                if (parameter is Window window)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Помилка збереження: " + ex.Message;
            }
        }

        private void ExecuteCancel(object? parameter)
        {
            if (parameter is Window window)
            {
                window.DialogResult = false;
                window.Close();
            }
        }
    }
}
