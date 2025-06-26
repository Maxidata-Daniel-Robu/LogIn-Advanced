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

        // Event to notify View when login is finished (success or failure)
        public event EventHandler<bool>? LoginFinished; // bool: true=success, false=failure

        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    Console.WriteLine("Password in ViewModel: " + _password); // TEMP DEBUG
                    OnPropertyChanged();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));

                    if (!string.IsNullOrEmpty(value))
                        StartFadeOutTimer();
                    else
                        StopFadeOutTimer();
                }
            }
        }

        public Brush StatusColor
        {
            get => _statusColor;
            set
            {
                if (_statusColor != value)
                {
                    _statusColor = value;
                    OnPropertyChanged(nameof(StatusColor));
                }
            }
        }

        private double _statusOpacity = 1.0;
        public double StatusOpacity
        {
            get => _statusOpacity;
            set
            {
                if (_statusOpacity != value)
                {
                    _statusOpacity = value;
                    OnPropertyChanged(nameof(StatusOpacity));
                }
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(async parameter => await ExecuteLoginAsync(parameter), CanExecuteLogin);
            NavigateToRegisterCommand = new RelayCommand(_ =>
            {
                MainWindow.AppNavigationService.NavigateTo("Register");
            });
        }

        private void StartFadeOutTimer()
        {
            StopFadeOutTimer();

            _fadeOutTimer = new DispatcherTimer();
            _fadeOutTimer.Interval = TimeSpan.FromSeconds(3);
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
            if (_fadeOutTimer != null)
                _fadeOutTimer.Stop();

            const int steps = 20;
            const int delayPerStep = 100; // milliseconds

            for (int i = 0; i <= steps; i++)
            {
                StatusOpacity = 1.0 - (double)i / steps;
                OnPropertyChanged(nameof(StatusOpacity));
                await Task.Delay(delayPerStep);
            }

            StatusMessage = string.Empty;
            StatusOpacity = 1.0;
            OnPropertyChanged(nameof(StatusOpacity));
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

            await Task.Run(() =>
            {
                var user = UserStorage.GetUser(Username, Password);

                if (user == null)
                {
                    _loginAttempts++;
                    StatusMessage = "Invalid username or password.";
                    StatusColor = Brushes.Red;

                    if (_loginAttempts >= 3)
                    {
                        // Signal failure, View handles shutdown
                        LoginFinished?.Invoke(this, false);
                    }
                    return;
                }

                StatusMessage = "Login successful!";
                StatusColor = Brushes.Green;

                // Signal success, View handles shutdown
                LoginFinished?.Invoke(this, true);
            });
        }

        private bool CanExecuteLogin(object? parameter) => true;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
