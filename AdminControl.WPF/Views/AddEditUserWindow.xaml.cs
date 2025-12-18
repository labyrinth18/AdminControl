using AdminControl.WPF.ViewModels;
using AdminControl.DTO;
using System.Windows;

namespace AdminControl.WPF.Views
{
    public partial class AddEditUserWindow : Window
    {
        private readonly AddEditUserViewModel _viewModel;

        public AddEditUserWindow(AddEditUserViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            // Підписуємося на зміни паролю
            pbPassword.PasswordChanged += (s, e) => _viewModel.Password = pbPassword.Password;
            pbConfirmPassword.PasswordChanged += (s, e) => _viewModel.ConfirmPassword = pbConfirmPassword.Password;
        }

        public void SetMode(bool isEditMode, UserDto? userToEdit = null)
        {
            _viewModel.SetMode(isEditMode, userToEdit);
        }
    }
}
