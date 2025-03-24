using Microsoft.AspNetCore.SignalR;

namespace RemoteCheckup.DTOs
{
    [Serializable]
    public class DatabasesInfo
    {
        public List<DatabaseServerInfo> Servers { get; set; } = [];
    }

    [Serializable]
    public class DatabaseServerInfo
    {
        public string Type { get; set; } = "";
        public List<DatabaseInfo> Databases { get; set; } = [];
        public bool Online { get; set; } = false;
    }

    [Serializable]
    public class DatabaseInfo
    {
        public string Name { get; set; } = "";
        public List<TableInfo> Tables = [];
    }

    [Serializable]
    public class TableInfo
    {
        public string Name { get; set; } = "";
    }
}
