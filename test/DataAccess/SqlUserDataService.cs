using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
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
            (s.StartsWith("$2a$") || s.StartsWith("$2b$") ||
             s.StartsWith("$2y$") || s.StartsWith("$2x$"));

        public async Task<bool> AddUserAsync(UserModel user)
        {
            if (!IsBCryptHash(user.Password))
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("User_Ins", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Password", user.Password);

            var returnParam = cmd.Parameters.Add("RETURN_VALUE", SqlDbType.Int);
            returnParam.Direction = ParameterDirection.ReturnValue;

            await cmd.ExecuteNonQueryAsync();
            int rows = (int)(returnParam.Value ?? 0);
            return rows > 0;
        }

        public async Task<bool> DeleteUserAsync(string username)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("User_Del", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Username", username);

            var returnParam = cmd.Parameters.Add("RETURN_VALUE", SqlDbType.Int);
            returnParam.Direction = ParameterDirection.ReturnValue;

            await cmd.ExecuteNonQueryAsync();
            int rows = (int)(returnParam.Value ?? 0);
            return rows > 0;
        }

        public async Task<bool> UpdateUserAsync(UserModel user)
        {
            // ensure hash
            var pwToStore = IsBCryptHash(user.Password)
                ? user.Password
                : BCrypt.Net.BCrypt.HashPassword(user.Password);

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("User_Upd", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Password", pwToStore);

            var returnParam = cmd.Parameters.Add("RETURN_VALUE", SqlDbType.Int);
            returnParam.Direction = ParameterDirection.ReturnValue;

            await cmd.ExecuteNonQueryAsync();
            int rows = (int)(returnParam.Value ?? 0);
            return rows > 0;
        }

        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            var list = new List<UserModel>();
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(
                "SELECT Id, Username, Password, Description FROM Users", conn);

            using var rdr = await cmd.ExecuteReaderAsync();
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

        public async Task<bool> UserExistsAsync(string username)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Users WHERE Username = @u", conn);
            cmd.Parameters.AddWithValue("@u", username);

            var result = await cmd.ExecuteScalarAsync();
            return result != null && Convert.ToInt32(result) > 0;
        }

        public async Task<UserModel?> GetUserAsync(string username)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(
                "SELECT Id, Username, Password, Description FROM Users WHERE Username = @u", conn);
            cmd.Parameters.AddWithValue("@u", username);

            using var rdr = await cmd.ExecuteReaderAsync();
            if (await rdr.ReadAsync())
            {
                return new UserModel
                {
                    Id = rdr.GetInt32(0),
                    Username = rdr.GetString(1),
                    Password = rdr.GetString(2),
                    Description = rdr.IsDBNull(3) ? "" : rdr.GetString(3)
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

        public async Task<bool> UpdateUserDescriptionAsync(int id, string description)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(
                "UPDATE Users SET Description = @desc WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@desc", description);
            cmd.Parameters.AddWithValue("@id", id);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }
    }
}
