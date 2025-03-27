using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RemoteCheckup.DTOs
{
    [Serializable]
    public class LoginData
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = "";
        [JsonPropertyName("password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
        [JsonPropertyName("persist")]
        public bool RememberMe { get; set; } = false;
    }

    [Serializable]
    public class PasswordChangeData
    {
        [JsonPropertyName("old")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = "";
        [JsonPropertyName("new")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = "";
        [JsonPropertyName("new2")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation must match")]
        public string NewPassword2 { get; set; } = "";
    }
}