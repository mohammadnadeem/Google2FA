namespace UserApi.Models
{
    public class AuthenticateUserRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// Code Received on Google Authenticator App
        /// </summary>
        public string Google2FACode { get; set; }
    }
}