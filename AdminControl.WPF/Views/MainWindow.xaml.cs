using AdminControl.WPF.ViewModels;
using System.Windows;

namespace AdminControl.WPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
