using UserApi.Models;

namespace UserApi.Repository
{
    public class UserRepository : IUserRepository
    {
        public UserRepository()
        {
            using (var context = new Google2FAContext())
            {
                var users = new List<User>()
                {
                    new User
                    {
                        Username = "Admin",
                        Password = "12345"
                    },
                    new User
                    {
                        Username = "Shahzeb",
                        Password = "12345"
                    }
                };

                context.Users.AddRange(users);
                context.SaveChanges();
            }
        }

        public bool IsValidUser(string username, string password)
        {
            using (var context = new Google2FAContext())
            {
                var users = context.Users.ToList();

                return users.Any(user => user.Username.Equals(username) && user.Password.Equals(password));
            }
        }
    }
}
