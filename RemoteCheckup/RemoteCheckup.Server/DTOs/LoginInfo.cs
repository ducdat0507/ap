using Microsoft.AspNetCore.SignalR;
using System.Text.Json.Serialization;

namespace RemoteCheckup.DTOs
{
    [Serializable]
    public class LoginInfo
    {
        [JsonPropertyName("username")]
        public required string Username { get; set; }
        [JsonPropertyName("password")]
        public required string Password { get; set; }
    }
}