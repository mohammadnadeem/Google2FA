using Googl2FA.Models;

namespace Googl2FA.Repository
{
    public interface IUserApiClient
    {
        Task<AuthenticateUserResult> AuthenticateUserAsync(string username, string password, string code);
    }
}