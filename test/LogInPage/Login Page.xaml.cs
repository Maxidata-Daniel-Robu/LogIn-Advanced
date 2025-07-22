using System.Windows;
using System.Windows.Controls;
using test.ViewModels;

namespace test.LogInPage
{
    public partial class Login_Page : Page
    {
        public Login_Page()
        {
            InitializeComponent();
            DataContext = new LoginViewModel(MainWindow.AppNavigationService);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
                vm.Password = pb.Password;
        }
    }
}
