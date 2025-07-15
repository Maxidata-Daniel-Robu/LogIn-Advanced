using System.Windows.Controls;
using test.Services;
using test.ViewModels;

namespace test
{
    public partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
            DataContext = new WelcomeViewModel(MainWindow.AppNavigationService);
        }
    }
}
