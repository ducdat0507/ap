
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace RemoteCheckup.Models
{
    public class DatabaseTarget
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public required DatabaseType DatabaseType { get; set; }
        public required string ConnectionString { get; set; }
        public required string ConnectionSecret { get; set; }
    }

    public enum DatabaseType 
    {
        MySQL = 0
    }
}