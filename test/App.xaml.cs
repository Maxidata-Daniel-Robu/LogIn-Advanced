using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using test.Services;

namespace test
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            var services = new ServiceCollection();

            string storage = config["AppSettings:Storage"] ?? "Json";
            string connectionString = config["AppSettings:ConnectionString"] ?? "";

            // Perform migration if switching to SQL
            if (storage == "Sql")
            {
                string jsonFilePath = Path.Combine(AppContext.BaseDirectory, "Data", "user.json");
                if (File.Exists(jsonFilePath))
                {
                    DataMigrationService.MigrateJsonToSql(jsonFilePath, connectionString);
                }
            }

            if (storage == "Manual")
            {
                // Ensure DB and table exist
                SqlSchemaInitializer.EnsureDatabaseAndSchemaAsync(connectionString).Wait(); // Synchronous wait for async method

                services.AddSingleton<IUserDataService>(new ManualUserDataService(connectionString));
            }

            ServiceProvider = services.BuildServiceProvider();

            base.OnStartup(e);

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}