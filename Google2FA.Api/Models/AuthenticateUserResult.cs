﻿namespace UserApi.Models
{
    public class AuthenticateUserResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}