using System.ComponentModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.Data;  // Your DbContext namespace
using test.Models;

namespace test.Models
{
    // Interface declaration for IUserStorage
    //do it in a file of its own 
    public interface IUserStorage : INotifyPropertyChanged
    {
        bool UserExists(string username);
        User? ValidateUser(string username, string password);
        void AddUser(User newUser);
    }

    // UserStorage implementation using EF Core
    public class UserStorage : IUserStorage
    {
        private readonly AppDbContext _dbContext;

        public event PropertyChangedEventHandler? PropertyChanged;

        public UserStorage(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool UserExists(string username)
        {
            return _dbContext.Users.Any(u => u.Username == username);
        }

        public User? ValidateUser(string username, string password)
        {
            // TODO: Ideally verify hashed password here
            return _dbContext.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public void AddUser(User newUser)
        {
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();
            OnPropertyChanged(nameof(UserExists));
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
