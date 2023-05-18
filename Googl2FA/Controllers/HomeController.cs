using Googl2FA.Config;
using Googl2FA.Models;
using Googl2FA.Repository;
using Google.Authenticator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text;

namespace Googl2FA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Google2FAConfig _google2FAConfig;
        private readonly IUserRepository _userRepository;

        public HomeController(ILogger<HomeController> logger, IOptions<Google2FAConfig> options, IUserRepository userRepository)
        {
            _logger = logger;
            _google2FAConfig = options.Value;
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Username") == null || HttpContext.Session.GetString("IsValidTwoFactorAuthentication") == null
                || !Convert.ToBoolean(HttpContext.Session.GetString("IsValidTwoFactorAuthentication")))
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult Login()
        {
            HttpContext.Session.SetString("UserName", "");
            HttpContext.Session.SetString("IsValidTwoFactorAuthentication", "false");

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            bool status = false;

            if (HttpContext.Session.GetString("Username") == null || HttpContext.Session.GetString("IsValidTwoFactorAuthentication") == null 
                || !Convert.ToBoolean(HttpContext.Session.GetString("IsValidTwoFactorAuthentication")))
            {                
                string UserUniqueKey = login.Username + _google2FAConfig.AuthKey;
                                
                if (_userRepository.IsValidUser(login.Username, login.Password))
                {
                    HttpContext.Session.SetString("Username", login.Username);

                    //Two Factor Authentication Setup
                    TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
                    var setupInfo = TwoFacAuth.GenerateSetupCode(_google2FAConfig.Issuer, login.Username, ConvertSecretToBytes(UserUniqueKey, false), 300);
                    HttpContext.Session.SetString("UserUniqueKey", UserUniqueKey);
                    ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
                    ViewBag.SetupCode = setupInfo.ManualEntryKey;
                    status = true;
                }
                else
                {
                    HttpContext.Session.SetString("InvalidCodeErrorMessage", "Invalid Credentials!!!!");
                }
            }
            else
            {                
                return RedirectToAction("Index");
            }
            ViewBag.Status = status;
            return View();
        }

        private static byte[] ConvertSecretToBytes(string secret, bool secretIsBase32) =>
           secretIsBase32 ? Base32Encoding.ToBytes(secret) : Encoding.UTF8.GetBytes(secret);

        public ActionResult TwoFactorAuthenticate(string CodeDigit)
        {
            var token = CodeDigit;
            TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
            string UserUniqueKey = HttpContext.Session.GetString("UserUniqueKey");
            UserUniqueKey = UserUniqueKey ?? "Adminsomerandomkey";
            bool isValid = TwoFacAuth.ValidateTwoFactorPIN(UserUniqueKey, token, false);
            if (isValid)
            {
                HttpContext.Session.SetString("IsValidTwoFactorAuthentication", "true");
                HttpContext.Session.SetString("InvalidCodeErrorMessage", "");
                return RedirectToAction("Index");
            }
            HttpContext.Session.SetString("InvalidCodeErrorMessage", "Google Two Factor PIN is expired or wrong");            
            return RedirectToAction("Login");
        }

        public ActionResult Logoff()
        {
            HttpContext.Session.SetString("UserName", "");
            HttpContext.Session.SetString("IsValidTwoFactorAuthentication", "false");

            return RedirectToAction("Login");
        }
    }
}