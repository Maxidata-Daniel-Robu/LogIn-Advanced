using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using test.Models;
using test.Services;

namespace test.DataAccess
{
    public class JsonUserDataService : IUserDataService
    {
        private readonly string _filePath;

        public JsonUserDataService()
            : this(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "user.json")) { }

        public JsonUserDataService(string filePath)
        {
            _filePath = Path.GetFullPath(filePath);
        }

        private static bool IsBCryptHash(string s) =>
            s is { Length: > 4 } &&
            (s.StartsWith("$2a$") || s.StartsWith("$2b$") ||
             s.StartsWith("$2y$") || s.StartsWith("$2x$"));

        public async Task<bool> AddUserAsync(UserModel user)
        {
            var users = await GetAllUsersAsync();
            if (users.Any(u => u.Username == user.Username))
                return false;

            user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
            user.Password = IsBCryptHash(user.Password)
                ? user.Password
                : BCrypt.Net.BCrypt.HashPassword(user.Password);

            users.Add(user);
            await SaveUsersAsync(users);
            return true;
        }

        public async Task<bool> DeleteUserAsync(string username)
        {
            var users = await GetAllUsersAsync();
            var existing = users.FirstOrDefault(u => u.Username == username);
            if (existing == null) return false;

            users.Remove(existing);
            await SaveUsersAsync(users);
            return true;
        }

        public async Task<bool> UpdateUserAsync(UserModel user)
        {
            var users = await GetAllUsersAsync();
            var existing = users.FirstOrDefault(u => u.Id == user.Id);
            if (existing == null) return false;

            // update all fields
            existing.Username = user.Username;
            existing.Description = user.Description;
            existing.Password = IsBCryptHash(user.Password)
                ? user.Password
                : BCrypt.Net.BCrypt.HashPassword(user.Password);

            await SaveUsersAsync(users);
            return true;
        }

        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            if (!File.Exists(_filePath))
                return new List<UserModel>();

            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<UserModel>>(json)
                   ?? new List<UserModel>();
        }

        public async Task<UserModel?> GetUserAsync(string username)
        {
            var users = await GetAllUsersAsync();
            return users.FirstOrDefault(u => u.Username == username);
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            var users = await GetAllUsersAsync();
            return users.Any(u => u.Username == username);
        }

        public async Task<bool> VerifyUserPasswordAsync(string username, string password)
        {
            var user = await GetUserAsync(username);
            if (user == null) return false;

            if (IsBCryptHash(user.Password))
                return BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (password == user.Password)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                await UpdateUserAsync(user);
                return true;
            }
            return false;
        }

        // no longer needed separately, kept for interface:
        public Task<bool> UpdateUserDescriptionAsync(int id, string description)
            => UpdateUserAsync(new UserModel { Id = id, Username = "", Password = "", Description = description });

        private async Task SaveUsersAsync(List<UserModel> users)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}
