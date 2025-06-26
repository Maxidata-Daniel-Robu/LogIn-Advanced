using System.Windows.Input;
using test.Commands;
using test.Services;

namespace test.ViewModels
{
    public class WelcomeViewModel
    {
        public ICommand NavigateCommand { get; }

        public WelcomeViewModel()
        {
            NavigateCommand = new RelayCommand(_ =>
            {
                MainWindow.AppNavigationService.NavigateTo("Login");
            });
        }
    }
}
