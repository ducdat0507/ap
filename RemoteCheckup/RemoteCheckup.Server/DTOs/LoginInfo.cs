using Microsoft.AspNetCore.SignalR;
using System.Text.Json.Serialization;

namespace RemoteCheckup.DTOs
{
    [Serializable]
    public class LoginInfo
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}