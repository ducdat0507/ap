using System;
using RemoteCheckup.Models;

namespace RemoteCheckup.Server.DTOs
{
    [Serializable]
    public class PublicDatabaseTarget
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DatabaseType DatabaseType { get; set; }
        public string ConnectionString { get; set; }
        public string? ConnectionSecret { get; set; }

        public PublicDatabaseTarget(DatabaseTarget info) 
        {
            Id = info.Id;
            Name = info.Name;
            DatabaseType = info.DatabaseType;
            ConnectionString = info.ConnectionString;
        }
    }
}
