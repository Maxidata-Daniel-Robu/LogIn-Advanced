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
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly INavigationService _navigationService;
        private readonly IUserDataService _userDataService;

        private string _username = "";
        private string _password = "";
        private string _confirmPassword = "";
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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); }
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

        public ICommand RegisterCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public RegisterViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService
                ?? throw new ArgumentNullException(nameof(navigationService));

            _userDataService = App.ServiceProvider
                               .GetService(typeof(IUserDataService)) as IUserDataService
                               ?? throw new InvalidOperationException("UserDataService not registered.");

            RegisterCommand = new RelayCommand(async _ => await ExecuteRegisterAsync());
            NavigateBackCommand = new RelayCommand(_ => _navigationService.NavigateTo("Welcome"));
            NavigateToLoginCommand = new RelayCommand(_ => _navigationService.NavigateTo("Login"));
        }

        private async Task ExecuteRegisterAsync()
        {
            StatusColor = Brushes.LightSteelBlue;
            StatusMessage = "";

            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                StatusMessage = "All fields are required.";
                return;
            }

            if (Password != ConfirmPassword)
            {
                StatusMessage = "Passwords do not match.";
                StatusColor = Brushes.OrangeRed;
                return;
            }

            // your existing complexity checks...
            bool added = await _userDataService.AddUserAsync(new Models.UserModel
            {
                Username = Username,
                Password = Password
            });

            if (added)
            {
                StatusMessage = "Registration successful.";
                StatusColor = Brushes.LimeGreen;
                await Task.Delay(500);
                _navigationService.NavigateTo("Login");
            }
            else
            {
                StatusMessage = "Username already exists.";
                StatusColor = Brushes.OrangeRed;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
