using System;
using System.Data;
using Microsoft.Data.SqlClient; // ✅ Recommended
using System.Threading.Tasks;

namespace test.Services
{
    public static class SqlSchemaInitializer
    {
        public static async Task EnsureDatabaseAndSchemaAsync(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            string dbName = builder.InitialCatalog;
            string masterConnStr = connectionString.Replace(dbName, "master");

            // Step 1: Create database if it doesn't exist
            using (SqlConnection conn = new SqlConnection(masterConnStr))
            {
                await conn.OpenAsync();
                string createDbQuery = $@"
                    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{dbName}')
                    BEGIN
                        CREATE DATABASE [{dbName}];
                    END";
                SqlCommand cmd = new SqlCommand(createDbQuery, conn);
                await cmd.ExecuteNonQueryAsync();
            }

            // Step 2: Create Users table if it doesn't exist
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                string createTableQuery = @"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
                    BEGIN
                        CREATE TABLE Users (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            Username NVARCHAR(100) NOT NULL UNIQUE,
                            PasswordHash NVARCHAR(200) NOT NULL
                        );
                    END";
                SqlCommand cmd = new SqlCommand(createTableQuery, conn);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
