using System.Collections.Generic;
using System.Threading.Tasks;
using test.Models;

namespace test.Services
{
    public interface IUserDataService
    {
        Task<bool> AddUserAsync(UserModel user);
        Task<bool> DeleteUserAsync(string username);
        Task<bool> UpdateUserAsync(UserModel user);
        Task<List<UserModel>> GetAllUsersAsync();
        Task<UserModel?> GetUserAsync(string username);
        Task<bool> UserExistsAsync(string username);
        Task<bool> VerifyUserPasswordAsync(string username, string password);

        // ← NEW: update only the description field
        Task<bool> UpdateUserDescriptionAsync(int id, string description);
    }
}
