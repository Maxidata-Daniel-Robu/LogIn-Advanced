using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using test.Models;
using test.Services;

namespace test.DataAccess
{
    public class SqlUserDataService : IUserDataService
    {
        private readonly string _connectionString;

        public SqlUserDataService(string connectionString)
        {
            _connectionString = connectionString;
        }

        private static bool IsBCryptHash(string s) =>
            s is { Length: > 4 } &&
            (s.StartsWith("$2a$") || s.StartsWith("$2b$") || s.StartsWith("$2y$") || s.StartsWith("$2x$"));

        public async Task<bool> AddUserAsync(UserModel user)
        {
            if (!IsBCryptHash(user.Password))
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand(
                "INSERT INTO Users (Username, Password) VALUES (@u, @p)", conn);
            cmd.Parameters.AddWithValue("@u", user.Username);
            cmd.Parameters.AddWithValue("@p", user.Password);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteUserAsync(string username)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand("DELETE FROM Users WHERE Username = @u", conn);
            cmd.Parameters.AddWithValue("@u", username);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateUserAsync(UserModel user)
        {
            if (!IsBCryptHash(user.Password))
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand("UPDATE Users SET Password = @p WHERE Username = @u", conn);
            cmd.Parameters.AddWithValue("@u", user.Username);
            cmd.Parameters.AddWithValue("@p", user.Password);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            var list = new List<UserModel>();
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand("SELECT Username, Password FROM Users", conn);
            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                list.Add(new UserModel
                {
                    Username = rdr.GetString(0),
                    Password = rdr.GetString(1)
                });
            }
            return list;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @u", conn);
            cmd.Parameters.AddWithValue("@u", username);
            var result = await cmd.ExecuteScalarAsync();
            return result != null && Convert.ToInt32(result) > 0;
        }

        public async Task<UserModel?> GetUserAsync(string username)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand("SELECT Username, Password FROM Users WHERE Username = @u", conn);
            cmd.Parameters.AddWithValue("@u", username);
            using var rdr = await cmd.ExecuteReaderAsync();
            if (await rdr.ReadAsync())
            {
                return new UserModel
                {
                    Username = rdr.GetString(0),
                    Password = rdr.GetString(1)
                };
            }
            return null;
        }

        public async Task<bool> VerifyUserPasswordAsync(string username, string password)
        {
            var user = await GetUserAsync(username);
            if (user == null) return false;

            if (IsBCryptHash(user.Password))
                return BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (password == user.Password)
            {
                await UpdateUserAsync(new UserModel
                {
                    Username = username,
                    Password = BCrypt.Net.BCrypt.HashPassword(password)
                });
                return true;
            }
            return false;
        }

        // UserDisplayModel (UI binding-ready, no warning)
        public class UserDisplayModel
        {
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public string HashedPassword { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }

        public async Task<List<UserDisplayModel>> GetAllUserDisplayModelsAsync()
        {
            var users = new List<UserDisplayModel>();
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand("SELECT Id, Username, Password, Description FROM Users", conn);
            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                users.Add(new UserDisplayModel
                {
                    Id = rdr.GetInt32(0),
                    Username = rdr.GetString(1),
                    HashedPassword = rdr.GetString(2),
                    Description = rdr.IsDBNull(3) ? "" : rdr.GetString(3)
                });
            }
            return users;
        }

        public async Task<bool> UpdateUserDescriptionAsync(int id, string description)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand("UPDATE Users SET Description = @desc WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@desc", description);
            cmd.Parameters.AddWithValue("@id", id);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<string?> GetUsernameByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand("SELECT Username FROM Users WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString();
        }
    }
}
