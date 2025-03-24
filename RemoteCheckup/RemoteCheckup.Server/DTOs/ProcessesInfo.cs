using Microsoft.AspNetCore.SignalR;

namespace RemoteCheckup.DTOs
{
    [Serializable]
    public class ProcessesInfo
    {
        public List<ProcessInfo> Processes { get; set; } = [];
        public List<ServiceInfo> Services { get; set; } = [];
        public List<ActivePortInfo> Ports { get; set; } = [];
    }

    [Serializable]
    public class ProcessInfo
    {
        public int PID { get; set; } = 0;
        public string Name { get; set; } = "";
        public ulong MemoryUsage { get; set; } = 0;
        public float CPUUsage { get; set; } = 0;
    }

    [Serializable]
    public class ServiceInfo
    {
        public string Name { get; set; } = "";
        public ServiceStatus Status { get; set; } = 0;
    }

    [Serializable]
    public class ActivePortInfo
    {
        public bool IsTCP { get; set; } = false;
        public PortStatus Status { get; set; } = PortStatus.Unknown;
        public ushort Port { get; set; } = 0;
        public int? PID { get; set; } = 0;
    }

    public enum ServiceStatus
    {
        Unknown = 0,
        Inactive = 12,
        Running = 13,
        Completed = 11,
        Faulted = 666666,
    }

    public enum PortStatus
    {
        Unknown = 0,
        Inactive = 11,
        Listening = 13,
    }
}
