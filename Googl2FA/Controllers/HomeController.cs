using Googl2FA.Models;
using Googl2FA.Repository;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using System.Diagnostics;
using System.Text;

namespace Googl2FA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserApiService _userApiClient;

        public HomeController(ILogger<HomeController> logger, IUserApiService userApiClient)
        {
            _logger = logger;
            _userApiClient = userApiClient;
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
                var result = _userApiClient.AuthenticateUserAsync(login.Username, login.Password, "").Result;

                if (result.Success)
                {
                    HttpContext.Session.SetString("Username", login.Username);
                    HttpContext.Session.SetString("Password", login.Password);
                    HttpContext.Session.SetString("UserUniqueKey", result.UserUniqueKey);
                    ViewBag.BarcodeImageUrl = result.QrCodeSetupImageUrl;
                    ViewBag.SetupCode = result.ManualEntryKey;
                    status = true;
                }
                else
                {
                    HttpContext.Session.SetString("InvalidCodeErrorMessage", result.ErrorMessage);
                }
            }
            else
            {                
                return RedirectToAction("Index");
            }
            ViewBag.Status = status;
            return View();
        }



        public ActionResult TwoFactorAuthenticate(string CodeDigit)
        {
            string username = HttpContext.Session.GetString("Username");
            string password = HttpContext.Session.GetString("Password");
            
            var result = _userApiClient.AuthenticateUserAsync(username, password, CodeDigit).Result;

            if (result.Success)
            {
                HttpContext.Session.SetString("IsValidTwoFactorAuthentication", "true");
                HttpContext.Session.SetString("InvalidCodeErrorMessage", "");
                return RedirectToAction("Index");
            }
            HttpContext.Session.SetString("InvalidCodeErrorMessage", result.ErrorMessage);            
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