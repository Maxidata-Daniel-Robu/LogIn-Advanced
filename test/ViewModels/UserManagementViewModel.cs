// File: ViewModels/UserManagementViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Configuration;
using test.Commands;
using test.DataAccess;
using test.Models;
using test.Services;

namespace test.ViewModels
{
    public class UserManagementViewModel : INotifyPropertyChanged
    {
        private readonly IUserDataService _userService;
        private readonly INavigationService _navigationService;

        private ObservableCollection<UserModel> _users = new();
        private ObservableCollection<UserModel> _filteredUsers = new();
        private string _searchQuery = string.Empty;
        private UserModel? _selectedUser;

        private string _newUsername = string.Empty;
        private string _newPassword = string.Empty;
        private string _newDescription = string.Empty;

        public ObservableCollection<UserModel> FilteredUsers
        {
            get => _filteredUsers;
            set { _filteredUsers = value; OnPropertyChanged(); }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; FilterUsers(); }
        }

        public UserModel? SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(); }
        }

        public string NewUsername
        {
            get => _newUsername;
            set { _newUsername = value; OnPropertyChanged(); }
        }

        public string NewPassword
        {
            get => _newPassword;
            set { _newPassword = value; OnPropertyChanged(); }
        }

        public string NewDescription
        {
            get => _newDescription;
            set { _newDescription = value; OnPropertyChanged(); }
        }

        public ICommand RefreshCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand DeleteSelectedCommand { get; }
        public ICommand AddDescriptionCommand { get; }
        public ICommand NavigateBackCommand { get; }

        public UserManagementViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService
                ?? throw new ArgumentNullException(nameof(navigationService));

            var config = App.ServiceProvider.GetService(typeof(IConfiguration))
                         as IConfiguration
                         ?? throw new InvalidOperationException("Configuration not available");

            var storage = config["AppSettings:Storage"] ?? "Json";

            _userService = storage.Equals("Sql", StringComparison.OrdinalIgnoreCase)
                ? new SqlUserDataService(config["AppSettings:ConnectionString"]!)
                : new JsonUserDataService(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Data", "user.json"));

            RefreshCommand = new RelayCommand(async _ => await RefreshAsync());
            SaveChangesCommand = new RelayCommand(async _ => await SaveChangesAsync());
            AddUserCommand = new RelayCommand(async _ => await AddUserAsync());
            DeleteSelectedCommand = new RelayCommand(async _ => await DeleteAsync());
            AddDescriptionCommand = new RelayCommand(async _ => await AddDescriptionAsync());
            NavigateBackCommand = new RelayCommand(_ => _navigationService.NavigateTo("Welcome"));

            _ = RefreshAsync();
        }

        public async Task RefreshAsync()
        {
            var list = await _userService.GetAllUsersAsync();
            _users = new ObservableCollection<UserModel>(list);
            FilteredUsers = new ObservableCollection<UserModel>(_users);
        }

        private void FilterUsers()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
                FilteredUsers = new ObservableCollection<UserModel>(_users);
            else
            {
                var q = SearchQuery.ToLower();
                FilteredUsers = new ObservableCollection<UserModel>(
                    _users.Where(u =>
                        u.Id.ToString().Contains(q) ||
                        u.Username.ToLower().Contains(q) ||
                        u.Description.ToLower().Contains(q)));
            }
        }

        private async Task SaveChangesAsync()
        {
            foreach (var u in FilteredUsers)
                await _userService.UpdateUserAsync(u);

            MessageBox.Show("All changes saved.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            await RefreshAsync();
        }

        private async Task AddUserAsync()
        {
            if (string.IsNullOrWhiteSpace(NewUsername) ||
                string.IsNullOrWhiteSpace(NewPassword))
            {
                MessageBox.Show("Enter both username and password", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var added = await _userService.AddUserAsync(new UserModel
            {
                Username = NewUsername,
                Password = NewPassword,
                Description = ""
            });

            MessageBox.Show(
                added ? "User inserted" : "Username exists",
                "Insert User",
                MessageBoxButton.OK,
                added ? MessageBoxImage.Information : MessageBoxImage.Warning);

            if (added)
            {
                NewUsername = "";
                NewPassword = "";
                await RefreshAsync();
            }
        }

        private async Task DeleteAsync()
        {
            if (SelectedUser == null)
            {
                MessageBox.Show("Select a user first", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var answer = MessageBox.Show(
                $"Delete {SelectedUser.Username}?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (answer != MessageBoxResult.Yes) return;

            var deleted = await _userService.DeleteUserAsync(SelectedUser.Username);
            MessageBox.Show(
                deleted ? "User deleted" : "Delete failed",
                "Delete",
                MessageBoxButton.OK,
                deleted ? MessageBoxImage.Information : MessageBoxImage.Error);

            if (deleted) await RefreshAsync();
        }

        private async Task AddDescriptionAsync()
        {
            if (SelectedUser == null)
            {
                MessageBox.Show("Select a user first", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(NewDescription))
            {
                MessageBox.Show("Enter a description", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedUser.Description = NewDescription;
            var updated = await _userService.UpdateUserDescriptionAsync(SelectedUser.Id, NewDescription);

            MessageBox.Show(
                updated ? "Description added" : "Failed to add description",
                "Add Description",
                MessageBoxButton.OK,
                updated ? MessageBoxImage.Information : MessageBoxImage.Error);

            if (updated)
            {
                NewDescription = "";
                await RefreshAsync();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
