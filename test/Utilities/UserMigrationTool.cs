using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using test.Models;
using test.Data;
using Microsoft.EntityFrameworkCore;

namespace test.Utilities
{
    public static class UserMigrationTool
    {
        private static bool _hasRun = false;

        public static void ImportJsonUsersOnce(DbContextOptions<AppDbContext> options)
        {
            if (_hasRun) return;
            _hasRun = true;

            string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "user.json");

            if (!File.Exists(jsonPath))
                return;

            var json = File.ReadAllText(jsonPath);
            var jsonUsers = JsonConvert.DeserializeObject<List<User>>(json);

            if (jsonUsers == null || jsonUsers.Count == 0)
                return;

            using var db = new AppDbContext(options);

            foreach (var user in jsonUsers)
            {
                user.Username = user.Username.Trim().ToLower();

                if (!db.Users.Any(u => u.Username == user.Username))
                {
                    if (!user.Password.StartsWith("$2"))
                        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

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
