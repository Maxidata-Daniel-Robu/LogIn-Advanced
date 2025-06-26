using System.Windows.Controls;
using test.ViewModels;

namespace test
{
    public partial class WelcomePage : Page
    {
        public WelcomePage(Frame frame) 
        {
            InitializeComponent();
            DataContext = new WelcomeViewModel();
        }
    }
}
