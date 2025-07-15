using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace test.LogInPage
{
    public partial class Register_Page : Page
    {
        public Register_Page()
        {
            InitializeComponent();
        }

        private void LoginLink_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Login_Page());
        }
    }
}
