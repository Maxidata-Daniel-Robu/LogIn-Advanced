// LoginPage.xaml.cs
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using test.ViewModels;

namespace test
{
    public partial class LoginPage : Page
    {
        private readonly LoginViewModel _viewModel;

        public LoginPage()
        {
            InitializeComponent();

            _viewModel = DataContext as LoginViewModel ?? throw new InvalidOperationException("DataContext must be LoginViewModel");
            _viewModel.LoginFinished += ViewModel_LoginFinished;

            this.Unloaded += LoginPage_Unloaded; // Subscribe to Unloaded event
        }

        private async void ViewModel_LoginFinished(object? sender, bool isSuccess)
        {
            await Task.Delay(500);  // wait 0.5 seconds

            // Use the UI Dispatcher to shut down the application on the UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                Application.Current.Shutdown();
            });
        }

        private void LoginPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel.LoginFinished -= ViewModel_LoginFinished;
        }

        private void RegisterLink_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new RegisterPage());
        }
    }
}
