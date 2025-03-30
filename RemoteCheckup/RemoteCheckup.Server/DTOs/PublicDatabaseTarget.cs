using System;
using System.Text.Json.Serialization;
using RemoteCheckup.Models;

namespace RemoteCheckup.Server.DTOs
{
    [Serializable]
    public class PublicDatabaseTarget
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        [JsonPropertyName("type")]
        public DatabaseType DatabaseType { get; set; }
        [JsonPropertyName("connString")]
        public string ConnectionString { get; set; } = "";
        [JsonPropertyName("connSecret")]
        public string? ConnectionSecret { get; set; }

        public PublicDatabaseTarget() {}

        public PublicDatabaseTarget(DatabaseTarget info) 
        {
            Id = info.Id;
            Name = info.Name;
            DatabaseType = info.DatabaseType;
            ConnectionString = info.ConnectionString;
        }
    }
}
