namespace Googl2FA.Repository
{
    public interface IUserRepository
    {
        public bool IsValidUser(string username, string password);
    }
}
