using System.Collections.Generic;
using System.Threading.Tasks;
using test.Models;

namespace test.Services
{
    public interface IUserDataService
    {
        Task<List<User>> GetAllUsersAsync();


        Task<User?> GetUserByUsernameAsync(string username); // ✅ Returns null if not found

        Task AddUserAsync(User user);

        Task<bool> UserExistsAsync(string username);

        // ✅ Secure password comparison (hashing should be handled in the implementation)
        Task<bool> VerifyUserPasswordAsync(string username, string password);
    }
}
