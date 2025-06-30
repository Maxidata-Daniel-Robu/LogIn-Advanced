using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using test.Models; // Your User model namespace
using test.Data;   // Your DbContext namespace

namespace test.Utilities
{
    public static class UserMigrationTool
    {
        public static void ImportJsonUsersOnce()
        {
            // Path to your users.json file inside the Data folder of the output directory
            string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "users.json");

            if (!File.Exists(jsonPath))
                return;

            var json = File.ReadAllText(jsonPath);
            var jsonUsers = JsonConvert.DeserializeObject<List<User>>(json);

            if (jsonUsers == null || jsonUsers.Count == 0)
                return;

            using var db = new AppDbContext();

            foreach (var user in jsonUsers)
            {
                // Only add if username doesn't already exist in DB
                if (!db.Users.Any(u => u.Username == user.Username))
                {
                    db.Users.Add(new User
                    {
                        Username = user.Username,
                        Password = user.Password
                    });
                }
            }

            db.SaveChanges();
        }
    }
}
