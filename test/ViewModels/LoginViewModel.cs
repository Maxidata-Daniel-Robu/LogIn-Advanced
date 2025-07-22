using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using test.Commands;
using test.Services;

namespace test.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly INavigationService _navigationService;
        private readonly IUserDataService _userDataService;
        private int _failedAttempts;

        private string _username = "";
        private string _password = "";
        private string _statusMessage = "";
        private Brush _statusColor = Brushes.LightSteelBlue;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public Brush StatusColor
        {
            get => _statusColor;
            set { _statusColor = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        // Constructor now takes your navigation service
        public LoginViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService
                ?? throw new ArgumentNullException(nameof(navigationService));

            // Resolve data service from DI
            _userDataService = App.ServiceProvider.GetService(typeof(IUserDataService))
                               as IUserDataService
                               ?? throw new InvalidOperationException("UserDataService not registered");

            LoginCommand = new RelayCommand(async _ => await ExecuteLoginAsync());
            NavigateBackCommand = new RelayCommand(_ => _navigationService.NavigateTo("Welcome"));
            NavigateToRegisterCommand = new RelayCommand(_ => _navigationService.NavigateTo("Register"));
        }

        private async Task ExecuteLoginAsync()
        {
            // reset status styling
            StatusColor = Brushes.LightSteelBlue;
            StatusMessage = "";

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                StatusMessage = "All fields are required.";
                return;
            }

            bool verified = await _userDataService.VerifyUserPasswordAsync(Username, Password);

            if (verified)
            {
                StatusMessage = "Login successful.";
                StatusColor = Brushes.LimeGreen;
                _failedAttempts = 0;

                // small pause so user sees message
                await Task.Delay(500);

                // then navigate or shut down
                _navigationService.NavigateTo("HomePage");
            }
            else
            {
                _failedAttempts++;
                StatusMessage = "Invalid credentials.";
                StatusColor = Brushes.OrangeRed;

                if (_failedAttempts >= 3)
                {
                    MessageBox.Show("Too many wrong tries. Application will now close.");
                    Application.Current.Shutdown();
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
