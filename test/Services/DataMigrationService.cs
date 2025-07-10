using test.DataAccess;
using test.Models;
using test.Services;

namespace test.Services
{
    public static class DataMigrationService
    {
        public static void MigrateJsonToSql(string jsonFilePath, string connectionString)
        {
            // Load existing users from JSON
            var jsonService = new JsonUserDataService();
            var users = jsonService.GetAllUsers();

            // Insert users into SQL
            var sqlService = new ManualUserDataService(connectionString);
            foreach (var user in users)
            {
                if (!sqlService.UserExists(user.Username))
                {
                    sqlService.AddUser(user);
                }
            }
        }
    }
}