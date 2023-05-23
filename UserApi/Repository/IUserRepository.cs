using UserApi.Models;

namespace UserApi.Repository
{
    public interface IUserRepository
    {
        public User GetUser(string username, string password);
    }
}
