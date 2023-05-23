using Google.Authenticator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using UserApi.Config;
using UserApi.Models;
using UserApi.Repository;

namespace UserApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;
        private readonly Google2FAConfig _google2FAConfig;

        public UserController(ILogger<UserController> logger, IUserRepository userRepository, IOptions<Google2FAConfig> options)
        {
            _logger = logger;
            _userRepository = userRepository;
            _google2FAConfig = options.Value;
        }

        [HttpPost(Name = "AuthenticateUser")]
        public async Task<ActionResult> AuthenticateUser([FromBody] AuthenticateUserRequest request)
        {
            if (request == null)
                return BadRequest();

            var user = _userRepository.GetUser(request.UserName, request.Password);
            
            if (user == null)
                return Unauthorized(new AuthenticateUserResult { Success = false, ErrorMessage = "Invalid Credentails!" });
                        
            var UserUniqueKey = request.UserName + user.SecretKey;

            if (string.IsNullOrEmpty(request.Google2FACode))
            {
                TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
                var setupInfo = TwoFacAuth.GenerateSetupCode(_google2FAConfig.Issuer, request.UserName, ConvertSecretToBytes(UserUniqueKey, false), 300);
                return Ok(new AuthenticateUserResult
                {
                    Success = true,
                    ManualEntryKey = setupInfo.ManualEntryKey,
                    QrCodeSetupImageUrl = setupInfo.QrCodeSetupImageUrl,
                    UserUniqueKey = UserUniqueKey,
                    TwoFAStatus = user.TwoFAStatus
                });
            }
            else
            {
                TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
                bool isValid = TwoFacAuth.ValidateTwoFactorPIN(UserUniqueKey, request.Google2FACode, false);
                if(isValid)
                {
                    return Ok(new AuthenticateUserResult
                    {
                        Success = true,
                        UserUniqueKey = UserUniqueKey
                    });
                }
                else
                {
                    return Unauthorized(new AuthenticateUserResult { Success = false, ErrorMessage = "Google Two Factor PIN is expired or wrong!" });
                }
            }                
        }

        private static byte[] ConvertSecretToBytes(string secret, bool secretIsBase32) =>
   secretIsBase32 ? Base32Encoding.ToBytes(secret) : Encoding.UTF8.GetBytes(secret);
    }
}