// File: DataAccess/SqlUserDataService.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
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
            _connectionString = connectionString
                ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            var list = new List<UserModel>();
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand(
                "SELECT Id, Username, Password, Description FROM Users", conn);
            await using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                list.Add(new UserModel
                {
                    Id = rdr.GetInt32(0),
                    Username = rdr.GetString(1),
                    Password = rdr.GetString(2),
                    Description = rdr.IsDBNull(3) ? "" : rdr.GetString(3)
                });
            }
            return list;
        }

        public async Task<UserModel?> GetUserAsync(string username)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand(
                "SELECT Id, Username, Password, Description FROM Users WHERE Username = @u", conn);
            cmd.Parameters.AddWithValue("@u", username);
            await using var rdr = await cmd.ExecuteReaderAsync();
            if (!await rdr.ReadAsync()) return null;
            return new UserModel
            {
                Id = rdr.GetInt32(0),
                Username = rdr.GetString(1),
                Password = rdr.GetString(2),
                Description = rdr.IsDBNull(3) ? "" : rdr.GetString(3)
            };
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Users WHERE Username = @u", conn);
            cmd.Parameters.AddWithValue("@u", username);
            var result = await cmd.ExecuteScalarAsync();
            int count = 0;
            if (result != null && result != DBNull.Value)
                count = Convert.ToInt32(result);
            return count > 0;
        }

        public async Task<bool> VerifyUserPasswordAsync(string username, string password)
        {
            var u = await GetUserAsync(username);
            return u != null && u.Password == Hash(password);
        }

        public async Task<bool> AddUserAsync(UserModel user)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            if (await UserExistsAsync(user.Username))
                return false;

            await using var cmd = new SqlCommand(
                "INSERT INTO Users (Username, Password, Description) VALUES (@u, @p, @d)", conn);
            cmd.Parameters.AddWithValue("@u", user.Username);
            cmd.Parameters.AddWithValue("@p", Hash(user.Password));
            cmd.Parameters.AddWithValue("@d", user.Description);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteUserAsync(string username)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand(
                "DELETE FROM Users WHERE Username = @u", conn);
            cmd.Parameters.AddWithValue("@u", username);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateUserAsync(UserModel user)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand(
                @"UPDATE Users 
                  SET Username    = @u,
                      Password    = @p,
                      Description = @d 
                  WHERE Id = @id", conn);

            cmd.Parameters.AddWithValue("@u", user.Username);
            cmd.Parameters.AddWithValue("@p", Hash(user.Password));
            cmd.Parameters.AddWithValue("@d", user.Description);
            cmd.Parameters.AddWithValue("@id", user.Id);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateUserDescriptionAsync(int id, string description)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand(
                "UPDATE Users SET Description = @d WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@d", description);
            cmd.Parameters.AddWithValue("@id", id);
            return await cmd.ExecuteNonQueryAsync() > 0;
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
