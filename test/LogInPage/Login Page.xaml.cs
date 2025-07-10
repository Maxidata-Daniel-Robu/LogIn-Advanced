using System.Windows;
using System.Windows.Controls;
using test.ViewModels;

namespace test.LogInPage
{
    public partial class Login_Page : Page
    {
        private readonly LoginViewModel _viewModel;

        public Login_Page()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            _viewModel.LoginFinished += ViewModel_LoginFinished;
            DataContext = _viewModel;
        }

        private void ViewModel_LoginFinished(object? sender, bool success)
        {
            if (success)
            {
                // Navigate to main or home page after successful login
                MainWindow.AppNavigationService.NavigateTo("HomePage");
            }
            else
            {
                // Optional: stay on the page or handle failed login
            }
        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.AppNavigationService.NavigateTo("Register");
        }
    }
}
