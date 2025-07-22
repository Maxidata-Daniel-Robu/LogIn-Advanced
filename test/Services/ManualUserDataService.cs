using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test.Models;
using test.Services;

namespace test.Services
{
    public class ManualUserDataService : IUserDataService
    {
        // In-memory storage for manual/testing
        private readonly List<UserModel> _users = new();

        public Task<bool> AddUserAsync(UserModel user)
        {
            if (_users.Any(u => u.Username == user.Username))
                return Task.FromResult(false);

            user.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
            _users.Add(user);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteUserAsync(string username)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null) return Task.FromResult(false);
            _users.Remove(user);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateUserAsync(UserModel user)
        {
            var existing = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existing == null) return Task.FromResult(false);

            existing.Username = user.Username;
            existing.Password = user.Password;
            existing.Description = user.Description;
            return Task.FromResult(true);
        }

        public Task<List<UserModel>> GetAllUsersAsync()
        {
            // Return a deep clone if needed; for now, shallow copy
            return Task.FromResult(_users.ToList());
        }

        public Task<UserModel?> GetUserAsync(string username)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            return Task.FromResult(user);
        }

        public Task<bool> UserExistsAsync(string username)
        {
            bool exists = _users.Any(u => u.Username == username);
            return Task.FromResult(exists);
        }

        public Task<bool> VerifyUserPasswordAsync(string username, string password)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null) return Task.FromResult(false);
            bool ok = user.Password == password;
            return Task.FromResult(ok);
        }

        // -------------------------
        // Newly added to satisfy IUserDataService
        // -------------------------
        public Task<bool> UpdateUserDescriptionAsync(int id, string description)
        {
            var existing = _users.FirstOrDefault(u => u.Id == id);
            if (existing == null)
                return Task.FromResult(false);

            existing.Description = description;
            return Task.FromResult(true);
        }
    }
}
