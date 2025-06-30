using System;
using System.Threading.Tasks;
using System.Windows;
using test.Data;
using test.Services;

namespace test
{
    public partial class App : Application
    {
        public static IUserDataService? UserDataService { get; private set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var dbContext = new AppDbContext();

            // 🔁 Choose between SQL or JSON storage
            var selectedStorage = StorageType.Json;

            UserDataService = UserDataServiceFactory.Create(selectedStorage, dbContext);

            // 🔄 Optional migration from JSON to SQL
            await MigrateJsonToSqlAsync();

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private static async Task MigrateJsonToSqlAsync()
        {
            var jsonService = new JsonUserDataService();
            var sqlService = new SqlUserDataService(new AppDbContext());

            var users = await jsonService.GetAllUsersAsync();

            foreach (var user in users)
            {
                var existing = await sqlService.GetUserByUsernameAsync(user.Username);
                if (existing == null)
                {
                    await sqlService.AddUserAsync(user);
                }
            }
        }
    }
}
