using Microsoft.AspNetCore.SignalR;

namespace RemoteCheckup.DTOs
{
    [Serializable]
    public class ProcessesInfo
    {
        public List<ProcessInfo> Processes { get; set; } = [];
        public List<ServiceInfo> Services { get; set; } = [];
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

    public enum ServiceStatus
    {
        Unknown = 0,
        Inactive = 12,
        Running = 13,
        Completed = 11,
        Faulted = 666666,
    }
}
