using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RemoteCheckup.DTOs
{
    [Serializable]
    public class LoginData
    {
        [JsonPropertyName("username")]
        [Required]
        public string Username { get; set; } = "";
        [JsonPropertyName("password")]
        [Required][DataType(DataType.Password)]
        public string Password { get; set; } = "";
        [JsonPropertyName("persist")]
        [Required]
        public bool RememberMe { get; set; } = false;
    }

    [Serializable]
    public class PasswordChangeData
    {
        [JsonPropertyName("oldPassword")]
        [Required][DataType(DataType.Password)]
        public string OldPassword { get; set; } = "";
        [JsonPropertyName("newPassword")]
        [Required][DataType(DataType.Password)]
        public string NewPassword { get; set; } = "";
        [JsonPropertyName("newPassword2")]
        [Required][DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation must match")]
        public string NewPassword2 { get; set; } = "";
    }
}