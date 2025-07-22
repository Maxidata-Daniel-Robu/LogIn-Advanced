using System;
using System.Windows.Input;
using test.Commands;
using test.Services;

namespace test.ViewModels
{
    public class WelcomeViewModel
    {
        private readonly INavigationService _navigationService;

        public ICommand NavigateToLoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }
        public ICommand ManageUsersCommand { get; }

        public WelcomeViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService
                ?? throw new ArgumentNullException(nameof(navigationService));

            NavigateToLoginCommand = new RelayCommand(_ => _navigationService.NavigateTo("Login"));
            NavigateToRegisterCommand = new RelayCommand(_ => _navigationService.NavigateTo("Register"));
            ManageUsersCommand = new RelayCommand(_ => _navigationService.NavigateTo("UserManagement"));
        }
    }
}
