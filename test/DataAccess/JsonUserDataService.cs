using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using test.Models;
using test.Services;

namespace test.DataAccess
{
    public class JsonUserDataService : IUserDataService
    {
        private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "user.json");

        private List<UserModel> LoadUsers()
        {
            if (!File.Exists(_filePath)) return new List<UserModel>();
            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<UserModel>>(json) ?? new List<UserModel>();
        }

        private void SaveUsers(List<UserModel> users)
        {
            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public void AddUser(UserModel user)
        {
            var users = LoadUsers();
            users.Add(user);
            SaveUsers(users);
        }

        public bool UserExists(string username)
        {
            return LoadUsers().Any(u => u.Username == username);
        }

        public bool VerifyUserPassword(string username, string password)
        {
            var user = LoadUsers().FirstOrDefault(u => u.Username == username);
            return user != null && BCrypt.Net.BCrypt.Verify(password, user.Password);
        }

        public List<UserModel> GetAllUsers() => LoadUsers();

        public UserModel? GetUser(string username)
        {
            return LoadUsers().FirstOrDefault(u => u.Username == username);
        }
    }
}
