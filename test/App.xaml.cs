using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;
using test.Data;
using test.DataAccess;
using test.Services;
using test.Utilities;

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

            var serviceCollection = new ServiceCollection();

            string storage = config["AppSettings:Storage"] ?? "Json";
            string connectionString = config["AppSettings:ConnectionString"] ?? "";

            if (storage == "Sql")
            {
                serviceCollection.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(connectionString));
                serviceCollection.AddScoped<IUserDataService, SqlUserDataService>();
            }
            else
            {
                serviceCollection.AddScoped<IUserDataService, JsonUserDataService>();
            }

            serviceCollection.AddSingleton(config);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            if (storage == "Sql")
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer(connectionString);
                UserMigrationTool.ImportJsonUsersOnce(optionsBuilder.Options);
            }

            base.OnStartup(e);

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
