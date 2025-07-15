using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using test.DataAccess;
using test.Services;

namespace test
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            // 1. Load configuration
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string storage = config["AppSettings:Storage"] ?? "Json";
            string connStr = config["AppSettings:ConnectionString"] ?? string.Empty;
            string jsonFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "user.json");

            // 2. If SQL is requested, try to migrate JSON → SQL first
            if (storage.Equals("Sql", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(connStr))
                {
                    MessageBox.Show(
                        "ConnectionString is missing. Falling back to local JSON storage.",
                        "Configuration error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    storage = "Json";
                }
                else
                {
                    try
                    {
                        // run on a background thread so the splash/UX stays responsive
                        await Task.Run(() =>
                            DataMigrationService.MigrateJsonToSql(jsonFile, connStr));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Migration failed – the app will continue in JSON-only mode.\n\n{ex.Message}",
                            "Migration error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        storage = "Json";
                    }
                }
            }

            // 3. Build DI container *after* we know which storage we will use
            var services = new ServiceCollection()
                .AddSingleton<IConfiguration>(config);

            if (storage.Equals("Sql", StringComparison.OrdinalIgnoreCase))
            {
                services.AddSingleton<IUserDataService>(
                    _ => new SqlUserDataService(connStr));
            }
            else
            {
                services.AddSingleton<IUserDataService>(
                    _ => new JsonUserDataService(jsonFile));
            }

            ServiceProvider = services.BuildServiceProvider();

            // 4. Show main window (navigates to WelcomePage in its ctor)
            new MainWindow().Show();
        }
    }
}
