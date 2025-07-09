using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test.Data;
using test.Models;
using test.Services;

namespace test.DataAccess
{
    public class SqlUserDataService : IUserDataService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public SqlUserDataService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task AddUserAsync(User user)
        {
            await using var context = _contextFactory.CreateDbContext();
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Users.ToListAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            await using var context = _contextFactory.CreateDbContext();
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(string username)
        {
            await using var context = _contextFactory.CreateDbContext();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> VerifyUserPasswordAsync(string username, string password)
        {
            await using var context = _contextFactory.CreateDbContext();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
            return user != null && user.Password == password;
        }
    }
}