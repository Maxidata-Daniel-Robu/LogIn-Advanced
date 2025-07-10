using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using test.Commands;
using test.Services;

namespace test.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username = string.Empty;
        private string _password = string.Empty;
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

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(async _ => await ExecuteLoginAsync(), _ => true);
        }

        private async Task ExecuteLoginAsync()
        {
            StatusColor = Brushes.Red;
            StatusMessage = "";

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                StatusMessage = "Username and password are required.";
                LoginFinished?.Invoke(this, false);
                return;
            }

            var userDataService = App.ServiceProvider?.GetService(typeof(IUserDataService)) as IUserDataService;
            if (userDataService == null)
            {
                StatusMessage = "User service unavailable.";
                LoginFinished?.Invoke(this, false);
                return;
            }

            bool verified = userDataService.VerifyUserPassword(Username, Password);

            if (verified)
            {
                StatusMessage = "Login successful.";
                StatusColor = Brushes.Green;
                LoginFinished?.Invoke(this, true);
            }
            else
            {
                StatusMessage = "Invalid credentials.";
                LoginFinished?.Invoke(this, false);
            }

            await Task.Delay(1);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
