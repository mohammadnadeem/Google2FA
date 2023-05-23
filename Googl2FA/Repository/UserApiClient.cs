using Googl2FA.Models;
using Newtonsoft.Json;
using System.Text;

namespace Googl2FA.Repository
{
    public class UserApiClient : IUserApiClient
    {
        public async Task<AuthenticateUserResult> AuthenticateUserAsync(string username, string password, string code)
        {
            var request = new AuthenticateUserRequest
            {
                UserName = username,
                Password = password,
                Google2FACode = code
            };

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "https://localhost:7114/User";
            using var client = new HttpClient();

            var response = await client.PostAsync(url, data);

            var responseJson = response.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<AuthenticateUserResult>(responseJson);

            return result;
        }
    }
}
