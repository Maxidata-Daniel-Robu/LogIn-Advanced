using System.Collections.Generic;
using test.Models;

namespace test.Services
{
    public interface IUserDataService
    {
        bool UserExists(string username);
        bool VerifyUserPassword(string username, string password);
        void AddUser(UserModel user);
        List<UserModel> GetAllUsers();
        UserModel? GetUser(string username);
    }
}
