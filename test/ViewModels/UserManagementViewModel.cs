using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using test.Commands;
using test.DataAccess;
using test.Services;

namespace test.ViewModels
{
    public class UserManagementViewModel : INotifyPropertyChanged
    {
        private readonly SqlUserDataService _userService;
        private ObservableCollection<SqlUserDataService.UserDisplayModel> _users = new();
        private ObservableCollection<SqlUserDataService.UserDisplayModel> _filteredUsers = new();
        private string _searchQuery = string.Empty;
        private string _selectedUsername = string.Empty;

        private SqlUserDataService.UserDisplayModel? _selectedUser;
        public SqlUserDataService.UserDisplayModel? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                SelectedUsername = _selectedUser != null
                    ? $"Username: {_selectedUser.Username}"
                    : string.Empty;
            }
        }

        public ObservableCollection<SqlUserDataService.UserDisplayModel> FilteredUsers
        {
            get => _filteredUsers;
            set { _filteredUsers = value; OnPropertyChanged(); }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; FilterUsers(); }
        }

        public string SelectedUsername
        {
            get => _selectedUsername;
            set { _selectedUsername = value; OnPropertyChanged(); }
        }

        public ICommand SaveDescriptionsCommand { get; }

        public UserManagementViewModel()
        {
            var config = App.ServiceProvider.GetService(typeof(Microsoft.Extensions.Configuration.IConfiguration))
                        as Microsoft.Extensions.Configuration.IConfiguration;

            string connStr = config?["AppSettings:ConnectionString"] ?? "";
            _userService = new SqlUserDataService(connStr);

            SaveDescriptionsCommand = new RelayCommand(async _ => await SaveDescriptionsAsync());

            LoadUsers();
        }

        private async void LoadUsers()
        {
            var list = await _userService.GetAllUserDisplayModelsAsync();
            _users = new ObservableCollection<SqlUserDataService.UserDisplayModel>(list);
            FilteredUsers = new ObservableCollection<SqlUserDataService.UserDisplayModel>(_users);
        }

        private void FilterUsers()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
                FilteredUsers = new ObservableCollection<SqlUserDataService.UserDisplayModel>(_users);
            else
            {
                var lowerQuery = SearchQuery.ToLower();
                FilteredUsers = new ObservableCollection<SqlUserDataService.UserDisplayModel>(
                    _users.Where(u => u.Id.ToString().Contains(lowerQuery) ||
                                      (u.Description != null && u.Description.ToLower().Contains(lowerQuery))));
            }
        }

        private async Task SaveDescriptionsAsync()
        {
            foreach (var user in FilteredUsers)
            {
                await _userService.UpdateUserDescriptionAsync(user.Id, user.Description);
            }
            MessageBox.Show("Descriptions updated successfully.", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
