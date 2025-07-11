using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;
using test.DataAccess;
using test.Services;

namespace test
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var config = LoadConfiguration();

            string? storage = config["AppSettings:Storage"];
            string? connectionString = config["AppSettings:ConnectionString"];

            var services = new ServiceCollection();

            if (string.Equals(storage, "Sql", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    MessageBox.Show("Missing SQL connection string.");
                    Shutdown();
                    return;
                }

                services.AddSingleton<IUserDataService>(new SqlUserDataService(connectionString));
            }
            else
            {
                services.AddSingleton<IUserDataService, JsonUserDataService>();
            }

            ServiceProvider = services.BuildServiceProvider();

            base.OnStartup(e);
        }

        private static IConfiguration LoadConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}
