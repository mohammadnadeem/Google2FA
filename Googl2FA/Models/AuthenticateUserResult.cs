namespace Googl2FA.Models
{
    public class AuthenticateUserResult
    {
        public bool Success { get; set; }
        public string UserUniqueKey { get; set; }
        public string QrCodeSetupImageUrl { get; set; }
        public string ManualEntryKey { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public bool TwoFAStatus { get; set; }
    }
}