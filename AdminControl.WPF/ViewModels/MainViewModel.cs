using AdminControl.DAL;
using AdminControl.DTO;
using AdminControl.WPF.Infrastructure;
using AdminControl.WPF.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace AdminControl.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IAdminActionLogRepository _actionLogRepository;
        private readonly IServiceProvider _serviceProvider;
        
        private UserDto? _selectedUser;
        private RoleDto? _selectedRole;
        private UserDto? _currentUser;
        private string _currentView = "Users";
        private bool _isAdmin;
        private bool _isManager;

        public ICommand DeleteUserCommand { get; }
        public ICommand ReloadCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ShowUsersCommand { get; }
        public ICommand ShowRolesCommand { get; }
        public ICommand ShowLogsCommand { get; }
        
        public ICommand AddRoleCommand { get; }
        public ICommand EditRoleCommand { get; }
        public ICommand DeleteRoleCommand { get; }

        // Колекції
        public ObservableCollection<UserDto> Users { get; set; }
        public ObservableCollection<RoleDto> Roles { get; set; }
        public ObservableCollection<AdminActionLogDto> ActionLogs { get; set; }

        public UserDto? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public RoleDto? SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public UserDto? CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentUserInfo));
                UpdatePermissions();
            }
        }

        public string CurrentUserInfo => CurrentUser != null 
            ? $"{CurrentUser.FullName} ({CurrentUser.RoleName})" 
            : "Не авторизовано";

        public string CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsUsersView));
                OnPropertyChanged(nameof(IsRolesView));
                OnPropertyChanged(nameof(IsLogsView));
                
                //if (value == "Logs")
                //{
                //    LoadLogs();
                //}
            }
        }

        public bool IsUsersView => CurrentView == "Users";
        public bool IsRolesView => CurrentView == "Roles";
        public bool IsLogsView => CurrentView == "Logs";
        
        public bool IsAdmin
        {
            get => _isAdmin;
            set { _isAdmin = value; OnPropertyChanged(); }
        }

        public bool IsManager
        {
            get => _isManager;
            set { _isManager = value; OnPropertyChanged(); }
        }

        public bool CanEdit => IsAdmin || IsManager;
        
        public bool CanDelete => IsAdmin;

        public MainViewModel(
            IUserRepository userRepository, 
            IRoleRepository roleRepository, 
            IAdminActionLogRepository actionLogRepository,
            IServiceProvider serviceProvider)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _actionLogRepository = actionLogRepository;
            _serviceProvider = serviceProvider;

            Users = new ObservableCollection<UserDto>();
            Roles = new ObservableCollection<RoleDto>();
            ActionLogs = new ObservableCollection<AdminActionLogDto>();

            DeleteUserCommand = new RelayCommand(ExecuteDeleteUser, CanDeleteUser);
            ReloadCommand = new RelayCommand(_ => LoadData());
            AddUserCommand = new RelayCommand(ExecuteAddUser, _ => CanEdit);
            EditUserCommand = new RelayCommand(ExecuteEditUser, CanEditUser);
            LogoutCommand = new RelayCommand(ExecuteLogout);
            ShowUsersCommand = new RelayCommand(_ => CurrentView = "Users");
            ShowRolesCommand = new RelayCommand(_ => { CurrentView = "Roles"; LoadRolesAsync(); });
            ShowLogsCommand = new RelayCommand(_ => CurrentView = "Logs");
            
            AddRoleCommand = new RelayCommand(ExecuteAddRole, _ => IsAdmin);
            EditRoleCommand = new RelayCommand(ExecuteEditRole, _ => SelectedRole != null && IsAdmin);
            DeleteRoleCommand = new RelayCommand(ExecuteDeleteRole, _ => SelectedRole != null && IsAdmin);

            LoadData();
        }

        public void Initialize(UserDto user)
        {
            CurrentUser = user;
        }

        private void UpdatePermissions()
        {
            if (CurrentUser == null)
            {
                IsAdmin = false;
                IsManager = false;
                return;
            }

            IsAdmin = CurrentUser.RoleName?.ToLower() == "admin";
            IsManager = CurrentUser.RoleName?.ToLower() == "manager";

            OnPropertyChanged(nameof(CanEdit));
            OnPropertyChanged(nameof(CanDelete));
            CommandManager.InvalidateRequerySuggested();
        }

        private async void LoadData()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();
                Users.Clear();
                foreach (var user in users)
                {
                    Users.Add(user);
                }

                await LoadRolesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка завантаження даних: " + ex.Message, 
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadRolesAsync()
        {
            try
            {
                var roles = await _roleRepository.GetAllRolesAsync();
                Roles.Clear();
                foreach (var role in roles)
                {
                    Roles.Add(role);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка завантаження ролей: " + ex.Message, 
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //private async void LoadLogs()
        //{
        //    try
        //    {
        //        //var logs = await _actionLogRepository.GetAllLogsAsync();
        //        ActionLogs.Clear();
        //        foreach (var log in logs)
        //        {
        //            ActionLogs.Add(log);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Помилка завантаження логів: " + ex.Message, 
        //            "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        private bool CanDeleteUser(object? parameter)
        {
            return SelectedUser != null && CanDelete;
        }

        private async void ExecuteDeleteUser(object? parameter)
        {
            if (SelectedUser == null) return;

            if (CurrentUser != null && SelectedUser.UserID == CurrentUser.UserID)
            {
                MessageBox.Show("Ви не можете видалити власний обліковий запис!",
                    "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Ви впевнені, що хочете видалити користувача {SelectedUser.Login}?",
                "Підтвердження видалення",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _userRepository.DeleteUserAsync(SelectedUser.UserID);
                    Users.Remove(SelectedUser);
                    SelectedUser = null;
                    
                    MessageBox.Show("Користувача успішно видалено!", 
                        "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка видалення: " + ex.Message,
                        "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExecuteAddUser(object? parameter)
        {
            var addUserWindow = (AddEditUserWindow)_serviceProvider.GetService(typeof(AddEditUserWindow))!;
            addUserWindow.SetMode(isEditMode: false);
            
            if (addUserWindow.ShowDialog() == true)
            {
                LoadData(); 
            }
        }

        private bool CanEditUser(object? parameter)
        {
            return SelectedUser != null && CanEdit;
        }

        private void ExecuteEditUser(object? parameter)
        {
            if (SelectedUser == null) return;

            var editUserWindow = (AddEditUserWindow)_serviceProvider.GetService(typeof(AddEditUserWindow))!;
            editUserWindow.SetMode(isEditMode: true, SelectedUser);
            
            if (editUserWindow.ShowDialog() == true)
            {
                LoadData(); 
            }
        }

        private async void ExecuteAddRole(object? parameter)
        {
            var dialog = new InputDialog("Додавання ролі", "Введіть назву нової ролі:");
            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.ResponseText))
            {
                try
                {
                    var newRole = new RoleCreateDto { RoleName = dialog.ResponseText };
                    await _roleRepository.AddRoleAsync(newRole);
                    await LoadRolesAsync();
                    MessageBox.Show("Роль успішно додано!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка додавання ролі: " + ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void ExecuteEditRole(object? parameter)
        {
            if (SelectedRole == null) return;

            var dialog = new InputDialog("Редагування ролі", "Введіть нову назву ролі:", SelectedRole.RoleName);
            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.ResponseText) && dialog.ResponseText != SelectedRole.RoleName)
            {
                try
                {
                    var updateDto = new RoleUpdateDto 
                    { 
                        RoleID = SelectedRole.RoleID, 
                        RoleName = dialog.ResponseText 
                    };
                    await _roleRepository.UpdateRoleAsync(updateDto);
                    await LoadRolesAsync();
                    LoadData();
                    MessageBox.Show("Роль успішно оновлено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка оновлення ролі: " + ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void ExecuteDeleteRole(object? parameter)
        {
            if (SelectedRole == null) return;

            var result = MessageBox.Show(
                $"Ви впевнені, що хочете видалити роль '{SelectedRole.RoleName}'?\n\nУвага: якщо роль використовується користувачами, видалення не вдасться.",
                "Підтвердження видалення",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _roleRepository.DeleteRoleAsync(SelectedRole.RoleID);
                    await LoadRolesAsync();
                    SelectedRole = null;
                    MessageBox.Show("Роль успішно видалено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка видалення ролі: " + ex.Message + "\n\nМожливо, роль використовується користувачами.", 
                        "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExecuteLogout(object? parameter)
        {
            var result = MessageBox.Show(
                "Ви впевнені, що хочете вийти?",
                "Підтвердження виходу",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = (LoginWindow)_serviceProvider.GetService(typeof(LoginWindow))!;
                loginWindow.Show();

                foreach (Window window in Application.Current.Windows)
                {
                    if (window is Views.MainWindow)
                    {
                        window.Close();
                        break;
                    }
                }
            }
        }
    }
}
