// File: DataAccess/JsonUserDataService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using test.Models;
using test.Services;

namespace test.DataAccess
{
    public class JsonUserDataService : IUserDataService
    {
        private readonly string _filePath;

        public JsonUserDataService(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Must provide a path", nameof(filePath));
            _filePath = filePath;
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
        }

        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<UserModel>>(json)
                   ?? new List<UserModel>();
        }

        public async Task<UserModel?> GetUserAsync(string username)
        {
            var users = await GetAllUsersAsync();
            return users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> UserExistsAsync(string username)
            => (await GetUserAsync(username)) != null;

        public async Task<bool> VerifyUserPasswordAsync(string username, string password)
        {
            var u = await GetUserAsync(username);
            return u != null && u.Password == Hash(password);
        }

        public async Task<bool> AddUserAsync(UserModel user)
        {
            var users = await GetAllUsersAsync();
            if (users.Any(u =>
                u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
                return false;

            // assign next Id
            user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
            // hash before save
            user.Password = Hash(user.Password);
            users.Add(user);
            await SaveUsersAsync(users);
            return true;
        }

        public async Task<bool> DeleteUserAsync(string username)
        {
            var users = await GetAllUsersAsync();
            var toRemove = users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (toRemove == null) return false;

            users.Remove(toRemove);
            await SaveUsersAsync(users);
            return true;
        }

        public async Task<bool> UpdateUserAsync(UserModel user)
        {
            var users = await GetAllUsersAsync();
            var existing = users.FirstOrDefault(u => u.Id == user.Id);
            if (existing == null) return false;

            existing.Username = user.Username;
            existing.Description = user.Description;
            // if password changed (plain vs. hash), re-hash
            if (user.Password != existing.Password)
                existing.Password = Hash(user.Password);
            await SaveUsersAsync(users);
            return true;
        }

        public Task<bool> UpdateUserDescriptionAsync(int id, string description)
            => UpdateUserAsync(new UserModel { Id = id, Username = "", Password = "", Description = description });

        private async Task SaveUsersAsync(List<UserModel> users)
        {
            var opts = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(users, opts);
            await File.WriteAllTextAsync(_filePath, json);
        }

        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
