using System;
using System.Windows.Controls;

namespace test.Services
{
    public class NavigationService : INavigationService
    {
        private readonly Frame _frame;

        public NavigationService(Frame frame)
        {
            _frame = frame;
        }

        public void NavigateTo(string pageKey)
        {
            switch (pageKey)
            {
                case "Login":
                    _frame.Navigate(new LoginPage());
                    break;
                case "Register":
                    _frame.Navigate(new RegisterPage());
                    break;
                case "Welcome":
                    _frame.Navigate(new WelcomePage(_frame)); // Pass the required 'frame' parameter
                    break;
                default:
                    throw new ArgumentException($"Unknown page key: {pageKey}");
            }
        }
    }
}
