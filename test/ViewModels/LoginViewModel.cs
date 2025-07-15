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
        private string _username = "";
        private string _password = "";
        private string _statusMessage = "";
        private Brush _statusColor = Brushes.LightSteelBlue;
        private int _failedAttempts;

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

        public event EventHandler<bool>? LoginFinished;
        public event PropertyChangedEventHandler? PropertyChanged;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(async _ => await ExecuteLoginAsync());
        }

        private async Task ExecuteLoginAsync()
        {
            StatusColor = Brushes.LightSteelBlue;
            StatusMessage = "";

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                StatusMessage = "All fields are required.";
                return;
            }

            var userDataService = App.ServiceProvider?.GetService(typeof(IUserDataService)) as IUserDataService;
            if (userDataService == null)
            {
                StatusMessage = "User service unavailable.";
                return;
            }

            bool verified = await userDataService.VerifyUserPasswordAsync(Username, Password);

            if (verified)
            {
                StatusMessage = "Login successful.";
                StatusColor = Brushes.LimeGreen;
                _failedAttempts = 0;
                LoginFinished?.Invoke(this, true);
                await Task.Delay(500);
                Application.Current.Shutdown();
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
                else
                {
                    LoginFinished?.Invoke(this, false);
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
