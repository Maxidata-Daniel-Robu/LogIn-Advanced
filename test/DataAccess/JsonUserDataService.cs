using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using test.Models;

namespace test.Services
{
    public class JsonUserDataService : IUserDataService
    {
        private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, "Data", "user.json");

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    var dir = Path.GetDirectoryName(_filePath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    await File.WriteAllTextAsync(_filePath, "[]");
                    return new List<User>();
                }

                var json = await File.ReadAllTextAsync(_filePath);
                var users = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();

                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading user.json: {ex.Message}");
                return new List<User>();
            }
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            var users = await GetAllUsersAsync();
            username = username.Trim().ToLower();

            return users.FirstOrDefault(u => u.Username.ToLower() == username);
        }

        public async Task AddUserAsync(User user)
        {
            var users = await GetAllUsersAsync();

            string normalizedUsername = user.Username.Trim().ToLower();

            if (users.Any(u => u.Username.Trim().ToLower() == normalizedUsername))
                throw new InvalidOperationException($"User '{user.Username}' already exists.");

            // Normalize username and hash password
            user.Username = normalizedUsername;

            if (!user.Password.StartsWith("$2"))
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            users.Add(user);

            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            var users = await GetAllUsersAsync();
            string normalizedUsername = username.Trim().ToLower();

            return users.Any(u => u.Username.ToLower() == normalizedUsername);
        }

        public async Task<bool> VerifyUserPasswordAsync(string username, string plainPassword)
        {
            var users = await GetAllUsersAsync();
            string normalizedUsername = username.Trim().ToLower();

            var user = users.FirstOrDefault(u => u.Username.ToLower() == normalizedUsername);

            if (user == null)
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(plainPassword, user.Password);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // Possibly plain text — rehash if it matches
                if (user.Password == plainPassword)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(plainPassword);

                    var updatedJson = JsonConvert.SerializeObject(users, Formatting.Indented);
                    await File.WriteAllTextAsync(_filePath, updatedJson);

                    return true;
                }

                return false;
            }
        }
    }
}
