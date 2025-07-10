using System;
using System.Windows.Controls;
using test.LogInPage;      // for Login_Page

namespace test.Services
{
    public class NavigationService : INavigationService
    {
        private readonly Frame _frame;

        public NavigationService(Frame frame)
        {
            _frame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        public void NavigateTo(string pageKey)
        {
            switch (pageKey)
            {
                case "Login":
                    _frame.Navigate(new Login_Page()); // Use correct class name
                    break;
                case "Register":
                    _frame.Navigate(new RegisterPage()); // Use correct class name
                    break;
                case "Welcome":
                    _frame.Navigate(new WelcomePage(_frame));
                    break;
                default:
                    throw new ArgumentException($"No page found for key: {pageKey}", nameof(pageKey));
            }
        }
    }
}