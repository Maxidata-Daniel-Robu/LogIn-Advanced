using System.Windows;
using test.Services;

namespace test
{
    public partial class MainWindow : Window
    {
        public static INavigationService AppNavigationService { get; private set; } = null!;

        public MainWindow()
        {
            InitializeComponent();
            AppNavigationService = new NavigationService(MainFrame);
            MainFrame.Navigate(new WelcomePage());
        }
    }
}
