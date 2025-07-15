using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using test.ViewModels;

namespace test.LogInPage
{
    public partial class Login_Page : Page
    {
        public Login_Page()
        {
            InitializeComponent();
            // XAML already provides DataContext via Page.DataContext
        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Register_Page());
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
                vm.Password = pb.Password;
        }
    }
}
