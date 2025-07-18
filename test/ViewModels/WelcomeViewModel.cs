using System.Windows.Input;
using test.Commands;
using test.Services;

namespace test.ViewModels
{
    public class WelcomeViewModel
    {
        public ICommand NavigateCommand { get; }
        public ICommand ManageUsersCommand { get; }

        public WelcomeViewModel(INavigationService navigationService)
        {
            NavigateCommand = new RelayCommand(_ => navigationService.NavigateTo("Login"));
            ManageUsersCommand = new RelayCommand(_ => navigationService.NavigateTo("UserManagement"));
        }
    }
}
