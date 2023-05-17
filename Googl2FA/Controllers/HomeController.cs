using Googl2FA.Models;
using Google.Authenticator;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace Googl2FA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult Login()
        {
            //Session["UserName"] = null;
            //Session["IsValidTwoFactorAuthentication"] = null;
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            bool status = false;

            if (HttpContext.Session.GetString("Username") == null || HttpContext.Session.GetString("IsValidTwoFactorAuthentication") == null 
                || !Convert.ToBoolean(HttpContext.Session.GetString("IsValidTwoFactorAuthentication")))
            {
                string googleAuthKey = "somerandomkey";
                string UserUniqueKey = (login.Username + googleAuthKey);

                //Take UserName And Password As Static - Admin As User And 12345 As Password
                if (login.Username == "Admin" && login.Password == "12345")
                {
                    HttpContext.Session.SetString("Username", login.Username);

                    //Two Factor Authentication Setup
                    TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
                    var setupInfo = TwoFacAuth.GenerateSetupCode("nadeem.1359@gmail.com", login.Username, ConvertSecretToBytes(UserUniqueKey, false), 300);
                    HttpContext.Session.SetString("UserUniqueKey", UserUniqueKey);
                    ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
                    ViewBag.SetupCode = setupInfo.ManualEntryKey;
                    status = true;
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
                //HttpCookie TwoFCookie = new HttpCookie("TwoFCookie");
                //string UserCode = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(UserUniqueKey)));

                HttpContext.Session.SetString("IsValidTwoFactorAuthentication", "true");
                HttpContext.Session.SetString("InvalidCodeErrorMessage", "");
                return RedirectToAction("Index");
            }
            HttpContext.Session.SetString("InvalidCodeErrorMessage", "Google Two Factor PIN is expired or wrong");
            ViewBag.Message = "Google Two Factor PIN is expired or wrong";
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