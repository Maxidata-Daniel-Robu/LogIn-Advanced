using System.Windows;
using System.Windows.Controls;
using test.ViewModels;

namespace test.LogInPage
{
    public partial class Register_Page : Page
    {
        public Register_Page()
        {
            InitializeComponent();
            DataContext = new RegisterViewModel(MainWindow.AppNavigationService);
        }

        // wire up the two PasswordBoxes
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm && sender is PasswordBox pb)
                vm.Password = pb.Password;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm && sender is PasswordBox pb)
                vm.ConfirmPassword = pb.Password;
        }
    }
}
