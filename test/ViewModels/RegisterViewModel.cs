using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using test.Commands;
using test.Models;
using test.Services;
using test.DataAccess;

namespace test.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _statusMessage = string.Empty;
        private Brush _statusColor = Brushes.Red;

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
        public ICommand NavigateToLoginCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(async param => await ExecuteRegisterAsync(), _ => true);
            NavigateToLoginCommand = new RelayCommand(_ =>
            {
                MainWindow.AppNavigationService.NavigateTo("Login");
            });
        }

        private async Task ExecuteRegisterAsync()
        {
            StatusColor = Brushes.Red;
            StatusMessage = "";

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                StatusMessage = "All fields are required.";
                return;
            }

            if (!IsPasswordValid(Password))
            {
                StatusMessage = "Password must contain an uppercase letter, number, and special character.";
                return;
            }

            if (Password != ConfirmPassword)
            {
                StatusMessage = "Passwords do not match.";
                return;
            }

            var userDataService = App.ServiceProvider?.GetService(typeof(IUserDataService)) as IUserDataService;
            if (userDataService == null)
            {
                StatusMessage = "User service is unavailable.";
                return;
            }

            if (userDataService.UserExists(Username))
            {
                StatusMessage = "Username already taken.";
                return;
            }

            var passwordToStore = userDataService is SqlUserDataService
                ? BCrypt.Net.BCrypt.HashPassword(Password)
                : Password;

            var newUser = new UserModel { Username = Username, Password = passwordToStore };

            try
            {
                userDataService.AddUser(newUser);
                StatusMessage = "Registration successful!";
                StatusColor = Brushes.Green;
                await Task.Delay(500);
                MainWindow.AppNavigationService.NavigateTo("Login");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Registration failed: {ex.Message}";
            }

            await Task.Delay(1);
        }

        private static bool IsPasswordValid(string password)
        {
            return !string.IsNullOrWhiteSpace(password) &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsDigit) &&
                   password.Any(c => !char.IsLetterOrDigit(c));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
