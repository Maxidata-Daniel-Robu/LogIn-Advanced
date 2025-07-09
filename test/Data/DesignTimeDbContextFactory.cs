using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;

namespace test.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Default to UserAuthDB if no config file found
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=UserAuthDB;Trusted_Connection=True;";

            string configPath = Path.Combine(Directory.GetCurrentDirectory(), "test.runtimeconfig.json");

            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                if (json.Contains("app:connection"))
                {
                    var start = json.IndexOf("app:connection") + "app:connection".Length + 3;
                    var end = json.IndexOf('"', start);
                    connectionString = json.Substring(start, end - start);
                }
            }

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
