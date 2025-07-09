using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using test.Commands;
using test.Models;
using test.Services;

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
            RegisterCommand = new RelayCommand(async param => await ExecuteRegisterAsync(param), CanExecuteRegister);
            NavigateToLoginCommand = new RelayCommand(_ =>
            {
                MainWindow.AppNavigationService.NavigateTo("Login");
            });
        }

        private async Task ExecuteRegisterAsync(object? parameter)
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

            // ✅ FIXED: get the user data service via DI
            var userDataService = App.ServiceProvider?.GetRequiredService<IUserDataService>();
            if (userDataService == null)
            {
                StatusMessage = "User service is unavailable.";
                return;
            }

            if (await userDataService.UserExistsAsync(Username))
            {
                StatusMessage = "Username already taken.";
                return;
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);
            var newUser = new User { Username = Username, Password = hashedPassword };

            try
            {
                await userDataService.AddUserAsync(newUser);

                StatusMessage = "Registration successful!";
                StatusColor = Brushes.Green;

                await Task.Delay(500);
                MainWindow.AppNavigationService.NavigateTo("Login");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Registration failed: {ex.Message}";
            }

            await Task.Delay(1); // ensures async signature is valid
        }

        private static bool IsPasswordValid(string password)
        {
            return !string.IsNullOrWhiteSpace(password) &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsDigit) &&
                   password.Any(c => !char.IsLetterOrDigit(c));
        }

        private bool CanExecuteRegister(object? _) => true;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
