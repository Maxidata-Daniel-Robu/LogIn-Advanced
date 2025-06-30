using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using test.Commands;
using test.Models;

namespace test.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _statusMessage = string.Empty;
        private Brush _statusColor = Brushes.Red;
        private int _loginAttempts = 0;
        private DispatcherTimer? _fadeOutTimer;

        public event EventHandler<bool>? LoginFinished;

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
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value)) StartFadeOutTimer();
                else StopFadeOutTimer();
            }
        }

        public Brush StatusColor
        {
            get => _statusColor;
            set { _statusColor = value; OnPropertyChanged(); }
        }

        private double _statusOpacity = 1.0;
        public double StatusOpacity
        {
            get => _statusOpacity;
            set { _statusOpacity = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(async param => await ExecuteLoginAsync(param), CanExecuteLogin);
            NavigateToRegisterCommand = new RelayCommand(_ =>
            {
                MainWindow.AppNavigationService.NavigateTo("Register");
            });
        }

        private void StartFadeOutTimer()
        {
            StopFadeOutTimer();
            _fadeOutTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _fadeOutTimer.Tick += FadeOutTimer_Tick;
            _fadeOutTimer.Start();
        }

        private void StopFadeOutTimer()
        {
            if (_fadeOutTimer != null)
            {
                _fadeOutTimer.Tick -= FadeOutTimer_Tick;
                _fadeOutTimer.Stop();
                _fadeOutTimer = null;
            }
            StatusOpacity = 1.0;
        }

        private async void FadeOutTimer_Tick(object? sender, EventArgs e)
        {
            _fadeOutTimer?.Stop();
            for (int i = 0; i <= 20; i++)
            {
                StatusOpacity = 1.0 - (double)i / 20;
                await Task.Delay(100);
            }
            StatusMessage = string.Empty;
            StatusOpacity = 1.0;
        }

        private async Task ExecuteLoginAsync(object? parameter)
        {
            StatusOpacity = 1.0;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                StatusMessage = "Username and password are required.";
                StatusColor = Brushes.OrangeRed;
                return;
            }

            if (App.UserDataService != null)
            {
                var user = await App.UserDataService.GetUserByUsernameAsync(Username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(Password, user.Password))
                {
                    _loginAttempts++;
                    StatusMessage = "Invalid username or password.";
                    StatusColor = Brushes.Red;

                    if (_loginAttempts >= 3)
                        LoginFinished?.Invoke(this, false);

                    return;
                }

                StatusMessage = "Login successful!";
                StatusColor = Brushes.Green;
                LoginFinished?.Invoke(this, true);
            }
            else
            {
                StatusMessage = "User data service is unavailable.";
                StatusColor = Brushes.Red;
            }
        }

        private bool CanExecuteLogin(object? parameter) => true;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
