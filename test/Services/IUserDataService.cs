using System.Collections.Generic;
using System.Threading.Tasks;
using test.Models;

namespace test.Services
{
    public interface IUserDataService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByUsernameAsync(string username);
        Task AddUserAsync(User user);
        Task<bool> UserExistsAsync(string username);
    }
}
