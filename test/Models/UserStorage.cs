using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace test.Models
{
    public class UserStorage
    {
        private static string GetUserFilePath()
        {
            // Navigate from bin/debug/net8.0-windows/ up to project root
            string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
            string dataDir = Path.Combine(projectRoot, "Data");

            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);

            return Path.Combine(dataDir, "user.json");
        }

        public static List<User> LoadUsers()
        {
            string path = GetUserFilePath();
            if (!File.Exists(path))
                return new List<User>();

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        public static void SaveUsers(List<User> users)
        {
            string path = GetUserFilePath();
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        public static void AddUser(User newUser)
        {
            var users = LoadUsers();
            users.Add(newUser);
            SaveUsers(users);
        }

        public static User? GetUser(string username, string password)
        {
            var users = LoadUsers();
            return users.Find(u => u.Username == username && u.Password == password);
        }

        public static bool IsUsernameTaken(string username)
        {
            var users = LoadUsers();
            return users.Exists(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
    }
}
