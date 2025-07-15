using System.Collections.Generic;
using System.Threading.Tasks;
using test.Models;

namespace test.Services
{
    public interface IUserDataService
    {
        Task<bool> AddUserAsync(UserModel user);
        Task<bool> UserExistsAsync(string username);
        Task<bool> UpdateUserAsync(UserModel user);
        Task<bool> DeleteUserAsync(string username);
        Task<List<UserModel>> GetAllUsersAsync();
        Task<UserModel?> GetUserAsync(string username);
        Task<bool> VerifyUserPasswordAsync(string username, string password);
    }
}
