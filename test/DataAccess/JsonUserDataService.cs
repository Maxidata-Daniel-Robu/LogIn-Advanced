using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using test.Models;
using test.Services;

namespace test.Services
{
    public class JsonUserDataService : IUserDataService
    {
        private readonly string _jsonPath = Path.Combine("Data", "users.json");

        public async Task<List<User>> GetAllUsersAsync()
        {
            if (!File.Exists(_jsonPath))
                return new List<User>();

            var json = await File.ReadAllTextAsync(_jsonPath);
            return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var users = await GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                throw new Exception("User not found.");

            return user;
        }

        public async Task AddUserAsync(User user)
        {
            var users = await GetAllUsersAsync();
            users.Add(user);
            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            await File.WriteAllTextAsync(_jsonPath, json);
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            var users = await GetAllUsersAsync();
            return users.Any(u => u.Username == username);
        }
    }
}
