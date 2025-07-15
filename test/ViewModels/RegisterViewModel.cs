using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using test.Commands;
using test.DataAccess;
using test.Models;
using test.Services;

namespace test.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
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

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(async _ => await ExecuteRegisterAsync());
        }

        private async Task ExecuteRegisterAsync()
        {
            StatusColor = Brushes.OrangeRed;
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
                return;
            }

            if (!Regex.IsMatch(Password, @"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$"))
            {
                StatusMessage = "Password must contain upper-case, number, special char.";
                return;
            }

            var jsonService = new JsonUserDataService();

            if (await jsonService.UserExistsAsync(Username))
            {
                StatusMessage = "Username already exists.";
                return;
            }

            bool ok = await jsonService.AddUserAsync(new UserModel
            {
                Username = Username,
                Password = Password
            });

            if (ok)
            {
                StatusColor = Brushes.LimeGreen;
                StatusMessage = "Registration successful. Redirecting...";
                await Task.Delay(500);

                // go to login page after success
                if (Application.Current.MainWindow?.FindName("MainFrame") is Frame frame)
                    frame.Navigate(new LogInPage.Login_Page());
            }
            else
            {
                StatusMessage = "Registration failed.";
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
