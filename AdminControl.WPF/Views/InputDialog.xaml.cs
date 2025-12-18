using System.Windows;

namespace AdminControl.WPF.Views
{
    public partial class InputDialog : Window
    {
        public string Title { get; set; }
        public string Prompt { get; set; }
        public string ResponseText { get; set; }

        public InputDialog(string title, string prompt, string defaultValue = "")
        {
            Title = title;
            Prompt = prompt;
            ResponseText = defaultValue;

            InitializeComponent();
            DataContext = this;

            txtResponse.Focus();
            txtResponse.SelectAll();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            ResponseText = txtResponse.Text;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
