using Microsoft.EntityFrameworkCore;
using test.Data;
using test.Services;

namespace test.DataAccess
{
    public static class UserDataServiceFactory
    {
        public static IUserDataService CreateSqlService(IDbContextFactory<AppDbContext> contextFactory)
        {
            return new SqlUserDataService(contextFactory);
        }
    }
}
