using test;
using test.Data;
using test.Models;

namespace test.Services
{
    public enum StorageType
    {
        Json,
        Sql
    }

    public static class UserDataServiceFactory
    {
        public static IUserDataService Create(StorageType type, AppDbContext dbContext)
        {
            return type switch
            {
                StorageType.Json => new JsonUserDataService(),
                StorageType.Sql => new SqlUserDataService(dbContext),
                _ => throw new ArgumentOutOfRangeException(nameof(type), "Unsupported storage type")
            };
        }
    }
}
