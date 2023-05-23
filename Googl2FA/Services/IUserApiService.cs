using Googl2FA.Models;

namespace Googl2FA.Repository
{
    public interface IUserApiService
    {
        Task<AuthenticateUserResult> AuthenticateUserAsync(string username, string password, string code);
    }
}