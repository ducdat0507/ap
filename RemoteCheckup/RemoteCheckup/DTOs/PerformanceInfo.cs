using Microsoft.AspNetCore.SignalR;

namespace RemoteCheckup.Models
{
    [Serializable]
    public class PerformanceInfo
    {
        public List<CPUInfo> Processors { get; set; } = new();
        public MemoryInfo Memory { get; set; } = new();
        public List<DriveInfo> Drives { get; set; } = new();
    }

    [Serializable]
    public class CPUInfo
    {
        public float TotalUsage { get; set; } = 0;
        public List<float> Usage { get; set; } = new();
    }

    [Serializable]
    public class MemoryInfo
    {
        public ulong TotalPhys { get; set; } = 0;
        public ulong UsedPhys { get; set; } = 0;
        public ulong CommittedPhys { get; set; } = 0;
        public ulong TotalSwap { get; set; } = 0;
        public ulong UsedSwap { get; set; } = 0;
    }

    [Serializable]
    public class DriveInfo
    {
        public ulong ReadSpeed { get; set; } = 0;
        public ulong WriteSpeed { get; set; } = 0;
        public List<PartitionInfo> Partitions { get; set; } = new();
    }

    [Serializable]
    public class PartitionInfo
    {
        public ulong TotalBytes { get; set; } = 0;
        public ulong UsedBytes { get; set; } = 0;
    }

}
