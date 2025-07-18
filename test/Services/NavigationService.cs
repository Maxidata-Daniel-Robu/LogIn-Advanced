using System;
using System.Windows.Controls;
using test.LogInPage; // for Login_Page and Register_Page

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
                    _frame.Navigate(new Login_Page());
                    break;
                case "Register":
                    _frame.Navigate(new Register_Page());
                    break;
                case "UserManagement":
                    _frame.Navigate(new UserManagement.UserManagementPage());
                    break;
                case "Welcome":
                case "HomePage":
                    _frame.Navigate(new WelcomePage());
                    break;
                default:
                    throw new ArgumentException($"No page found for key: {pageKey}", nameof(pageKey));
            }
        }
    }
}
