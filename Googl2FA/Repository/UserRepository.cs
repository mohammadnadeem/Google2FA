using NuGet.Protocol.Plugins;

namespace Googl2FA.Repository
{
    public class UserRepository : IUserRepository
    {
        public UserRepository()
        {

        }
        public bool IsValidUser(string username, string password)
        {
            if (username == "Admin" && password == "12345")
                return true;

            return false;
        }
    }
}
