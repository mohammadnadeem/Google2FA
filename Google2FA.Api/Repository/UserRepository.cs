using Microsoft.EntityFrameworkCore;
using UserApi.Models;

namespace UserApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _userDbContext;

        public UserRepository(UserDbContext userDbContext)
        {
            _userDbContext = userDbContext;
            _userDbContext.Database.EnsureCreated();
        }

        public bool IsValidUser(string username, string password)
        {
            var users = _userDbContext.Users.ToList();

            return users.Any(user => user.Username.Equals(username) && user.Password.Equals(password));            
        }
    }
}
