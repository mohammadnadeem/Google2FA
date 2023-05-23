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

        public User GetUser(string username, string password)
        {
            var users = _userDbContext.Users.ToList();

            return users.FirstOrDefault(user => string.Equals(user.Username, username) && string.Equals(user.Password, password));            
        }
    }
}
