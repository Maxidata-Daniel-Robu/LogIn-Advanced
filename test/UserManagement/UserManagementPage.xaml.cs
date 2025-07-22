using System.Windows.Controls;
using test.ViewModels;

namespace test.UserManagement
{
    public partial class UserManagementPage : Page
    {
        public UserManagementPage()
        {
            InitializeComponent();
            DataContext = new UserManagementViewModel(MainWindow.AppNavigationService);
        }
    }
}
