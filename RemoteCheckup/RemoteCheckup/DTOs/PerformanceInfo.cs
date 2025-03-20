using Microsoft.AspNetCore.SignalR;

namespace RemoteCheckup.Models
{
    [Serializable]
    public class PerformanceInfo
    {
        public List<CPUInfo> Processors { get; set; } = new();
        public MemoryInfo Memory { get; set; } = new();
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
        public ulong TotalBytes { get; set; } = 0;
        public ulong UsedBytes { get; set; } = 0;
        internal ulong CommittedBytes { get; set; } = 0;
    }

}
