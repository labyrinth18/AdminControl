using AdminControl.BLL.Interfaces;
using AdminControl.WPF.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
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
                ValidateLogin(); 
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

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        private bool CanExecuteLogin(object parameter)
        {
            return !HasErrors && !IsBusy;
        }

        private async void ExecuteLogin(object parameter)
        {
            if (parameter is not PasswordBox passwordBox)
                return;

            IsBusy = true;
            ErrorMessage = "";

            try
            {
                string password = passwordBox.Password;


                var user = await _authService.AuthenticateAsync(Login, password);

                
                OpenMainWindow();
                RequestClose?.Invoke(); 
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message; 
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OpenMainWindow()
        {
            
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ValidateLogin()
        {
            ClearErrors(nameof(Login));
            if (string.IsNullOrWhiteSpace(Login))
            {
                AddError("Логін не може бути порожнім", nameof(Login));
            }
        }
    }
}