using AdminControl.WPF.ViewModels;
using System.Windows;

namespace AdminControl.WPF.Views 
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel; 

            // Закриття вікна за запитом ViewModel
            _viewModel.RequestClose += () => this.Close();
        }
    }
}
