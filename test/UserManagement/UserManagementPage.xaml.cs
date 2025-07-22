using System.Windows.Controls;
using test.ViewModels;

namespace test.UserManagement
{
    public partial class UserManagementPage : Page
    {
        private readonly UserManagementViewModel _vm;

        public UserManagementPage()
        {
            InitializeComponent();
            _vm = new UserManagementViewModel();
            DataContext = _vm;

            // refresh every time the page loads (e.g., after navigation)
            Loaded += (_, __) => _ = _vm.RefreshAsync();
        }
    }
}
