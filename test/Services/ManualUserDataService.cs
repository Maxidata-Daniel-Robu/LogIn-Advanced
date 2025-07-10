using System;
using System.Collections.Generic;
using System.Data;
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

        public void AddUser(UserModel user)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand("INSERT INTO Users (Username, Password) VALUES (@Username, @Password)", connection);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.ExecuteNonQuery();
        }

        public bool UserExists(string username)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username", connection);
            command.Parameters.AddWithValue("@Username", username);
            var result = (int)command.ExecuteScalar();
            return result > 0;
        }

        public bool VerifyUserPassword(string username, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand("SELECT Password FROM Users WHERE Username = @Username", connection);
            command.Parameters.AddWithValue("@Username", username);
            var storedHash = command.ExecuteScalar() as string;
            return storedHash != null && BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        public List<UserModel> GetAllUsers()
        {
            var users = new List<UserModel>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand("SELECT Username, Password FROM Users", connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                users.Add(new UserModel
                {
                    Username = reader.GetString(0),
                    Password = reader.GetString(1)
                });
            }
            return users;
        }

        public UserModel? GetUser(string username)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand("SELECT Username, Password FROM Users WHERE Username = @Username", connection);
            command.Parameters.AddWithValue("@Username", username);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new UserModel
                {
                    Username = reader.GetString(0),
                    Password = reader.GetString(1)
                };
            }
            return null;
        }
    }
}
