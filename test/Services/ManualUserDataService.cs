using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using test.Models;

namespace test.Services
{
    public class ManualUserDataService : IUserDataService
    {
        private readonly string _connectionString;

        public ManualUserDataService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> AddUserAsync(UserModel user)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("INSERT INTO Users (Username, Password) VALUES (@Username, @Password)", connection);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Password", user.Password);

            int result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<bool> DeleteUserAsync(string username)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("DELETE FROM Users WHERE Username = @Username", connection);
            command.Parameters.AddWithValue("@Username", username);

            int result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<bool> UpdateUserAsync(UserModel user)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("UPDATE Users SET Password = @Password WHERE Username = @Username", connection);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Password", user.Password);

            int result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            var users = new List<UserModel>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("SELECT Username, Password FROM Users", connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new UserModel
                {
                    Username = reader.GetString(0),
                    Password = reader.GetString(1)
                });
            }

            return users;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username", connection);
            command.Parameters.AddWithValue("@Username", username);

            var result = await command.ExecuteScalarAsync();
            return result != null && Convert.ToInt32(result) > 0;
        }

        public async Task<UserModel?> GetUserAsync(string username)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("SELECT Username, Password FROM Users WHERE Username = @Username", connection);
            command.Parameters.AddWithValue("@Username", username);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UserModel
                {
                    Username = reader.GetString(0),
                    Password = reader.GetString(1)
                };
            }

            return null;
        }

        public async Task<bool> VerifyUserPasswordAsync(string username, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("SELECT Password FROM Users WHERE Username = @Username", connection);
            command.Parameters.AddWithValue("@Username", username);

            var result = await command.ExecuteScalarAsync();
            var storedHash = result as string;

            return storedHash != null && BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}
