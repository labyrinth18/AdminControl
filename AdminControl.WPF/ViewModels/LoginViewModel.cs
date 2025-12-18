using AdminControl.BLL.Interfaces;
using AdminControl.DTO;
using AdminControl.WPF.Infrastructure;
using AdminControl.WPF.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AdminControl.WPF.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly IServiceProvider _serviceProvider;

        private string _login = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isBusy;

        public event Action? RequestClose;

        public LoginViewModel(IAuthService authService, IServiceProvider serviceProvider)
        {
            _authService = authService;
            _serviceProvider = serviceProvider;
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        }

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();

                ClearErrors(nameof(Login));

                if (string.IsNullOrWhiteSpace(_login))
                {
                    AddError("Логін не може бути порожнім", nameof(Login));
                }
                else if (_login.Length < 3)
                {
                    AddError("Логін занадто короткий", nameof(Login));
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        private bool CanExecuteLogin(object? parameter)
        {
            return !IsBusy && !HasErrors && !string.IsNullOrWhiteSpace(Login);
        }

        private async void ExecuteLogin(object? parameter)
        {
            if (parameter is not PasswordBox passwordBox) return;

            if (string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                ErrorMessage = "Пароль не може бути порожнім";
                return;
            }

            if (passwordBox.Password.Length < 3)
            {
                ErrorMessage = "Пароль занадто короткий";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var user = await _authService.AuthenticateAsync(Login, passwordBox.Password);

                if (user.RoleName?.ToLower() != "admin" && user.RoleName?.ToLower() != "manager")
                {
                    MessageBox.Show(
                        "У вас немає прав доступу до адміністративної панелі!\n" +
                        "Тільки адміністратори та менеджери можуть увійти.",
                        "Доступ заборонено",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    IsBusy = false;
                    return;
                }

                OpenMainWindow(user);
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Невірний логін або пароль.\n" + ex.Message,
                    "Помилка входу",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OpenMainWindow(UserDto user)
        {
            var mainWindow = (MainWindow)_serviceProvider.GetService(typeof(Views.MainWindow))!;

            if (mainWindow.DataContext is MainViewModel mainVm)
            {
                mainVm.Initialize(user);
            }

            mainWindow.Show();
        }
    }
}
