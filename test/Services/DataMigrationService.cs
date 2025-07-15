using System;
using test.DataAccess;
using test.Models;

namespace test.Services
{
    public static class DataMigrationService
    {
        public static void MigrateJsonToSql(string jsonFilePath, string connectionString)
        {
            var jsonService = new JsonUserDataService(jsonFilePath);
            var sqlService = new SqlUserDataService(connectionString);

            var users = jsonService.GetAllUsersAsync().GetAwaiter().GetResult();

            foreach (var user in users)
            {
                bool exists = sqlService.UserExistsAsync(user.Username)
                                       .GetAwaiter().GetResult();
                if (!exists)
                {
                    sqlService.AddUserAsync(user)
                              .GetAwaiter().GetResult();
                }
            }
        }
    }
}
