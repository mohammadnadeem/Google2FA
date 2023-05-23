namespace UserApi.Repository
{
    public interface IUserRepository
    {
        public bool IsValidUser(string username, string password);
    }
}
