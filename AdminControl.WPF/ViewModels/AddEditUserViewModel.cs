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

                ClearErrors(nameof(Login));
                if (string.IsNullOrWhiteSpace(_login))
                    AddError("Логін не може бути порожнім", nameof(Login));
                else if (_login.Length < 3)
                    AddError("Логін має бути мінімум 3 символи", nameof(Login));
                else if (!System.Text.RegularExpressions.Regex.IsMatch(_login, @"^[a-zA-Z0-9_]+$"))
                    AddError("Тільки латиниця, цифри та _", nameof(Login));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();

                if (!_isEditMode) 
                {
                    ClearErrors(nameof(Password));
                    if (string.IsNullOrWhiteSpace(_password))
                        AddError("Пароль обов'язковий", nameof(Password));
                    else if (_password.Length < 6)
                        AddError("Мінімум 6 символів", nameof(Password));

                    
                    if (!string.IsNullOrEmpty(ConfirmPassword))
                    {
                        if (_password != ConfirmPassword)
                            AddError("Паролі не співпадають", nameof(ConfirmPassword));
                        else
                            ClearErrors(nameof(ConfirmPassword)); 
                    }
                }
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();

                if (!_isEditMode)
                {
                    ClearErrors(nameof(ConfirmPassword));
                    if (string.IsNullOrWhiteSpace(_confirmPassword))
                        AddError("Підтвердіть пароль", nameof(ConfirmPassword));
                    else if (_password != _confirmPassword)
                        AddError("Паролі не співпадають", nameof(ConfirmPassword));
                }
            }
        }
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();

            
                ClearErrors(nameof(FirstName));
                if (string.IsNullOrWhiteSpace(_firstName))
                    AddError("Ім'я не може бути порожнім", nameof(FirstName));
                else if (_firstName.Length < 2)
                    AddError("Ім'я повинно містити мінімум 2 символи", nameof(FirstName));
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();

            
                ClearErrors(nameof(LastName));
                if (string.IsNullOrWhiteSpace(_lastName))
                    AddError("Прізвище не може бути порожнім", nameof(LastName));
                else if (_lastName.Length < 2)
                    AddError("Прізвище повинно містити мінімум 2 символи", nameof(LastName));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();

             
                ClearErrors(nameof(Email));
                if (string.IsNullOrWhiteSpace(_email))
                    AddError("Email не може бути порожнім", nameof(Email));
                else if (!Regex.IsMatch(_email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    AddError("Невірний формат email", nameof(Email));
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();

                ClearErrors(nameof(PhoneNumber));
           
                if (!string.IsNullOrWhiteSpace(_phoneNumber) && !Regex.IsMatch(_phoneNumber, @"^[\+]?[0-9\s\-\(\)]+$"))
                {
                    AddError("Невірний формат телефону", nameof(PhoneNumber));
                }
            }
        }
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
                ClearErrors(nameof(Address)); 
            }
        }

        public string SelectedGender
        {
            get => _selectedGender;
            set
            {
                _selectedGender = value;
                OnPropertyChanged();
                ClearErrors(nameof(SelectedGender));
            }
        }
        public RoleDto? SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged();

                ClearErrors(nameof(SelectedRole));
                if (_selectedRole == null)
                {
                    AddError("Оберіть роль", nameof(SelectedRole));
                }
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

                ClearAllErrors();
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

                ClearAllErrors(); 
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

            if (string.IsNullOrWhiteSpace(FirstName))
                return "Ім'я не може бути порожнім";
            if (FirstName.Length < 2)
                return "Ім'я повинно містити мінімум 2 символи";

            if (string.IsNullOrWhiteSpace(LastName))
                return "Прізвище не може бути порожнім";
            if (LastName.Length < 2)
                return "Прізвище повинно містити мінімум 2 символи";

            if (string.IsNullOrWhiteSpace(Email))
                return "Email не може бути порожнім";
            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return "Невірний формат email";

            if (!string.IsNullOrWhiteSpace(PhoneNumber))
            {
                if (!Regex.IsMatch(PhoneNumber, @"^[\+]?[0-9\s\-\(\)]+$"))
                    return "Невірний формат номера телефону";
            }

            if (SelectedRole == null)
                return "Будь ласка, виберіть роль";

            return null; 
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
