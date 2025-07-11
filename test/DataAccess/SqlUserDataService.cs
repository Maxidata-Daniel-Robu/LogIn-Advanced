using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
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

        public void AddUser(UserModel user)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand("INSERT INTO Users (Username, Password) VALUES (@username, @password)", conn);
            cmd.Parameters.AddWithValue("@username", user.Username);
            cmd.Parameters.AddWithValue("@password", user.Password);
            cmd.ExecuteNonQuery();
        }

        public bool UserExists(string username)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @username", conn);
            cmd.Parameters.AddWithValue("@username", username);
            return (int)cmd.ExecuteScalar() > 0;
        }

        public bool VerifyUserPassword(string username, string password)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand("SELECT Password FROM Users WHERE Username = @username", conn);
            cmd.Parameters.AddWithValue("@username", username);
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string hash = reader.GetString(0);
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            return false;
        }

        public List<UserModel> GetAllUsers()
        {
            var users = new List<UserModel>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand("SELECT Username, Password FROM Users", conn);
            var reader = cmd.ExecuteReader();
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
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand("SELECT Username, Password FROM Users WHERE Username = @username", conn);
            cmd.Parameters.AddWithValue("@username", username);
            var reader = cmd.ExecuteReader();
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
