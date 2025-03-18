using Microsoft.AspNetCore.SignalR;

namespace RemoteCheckup.Models
{
    [Serializable]
    public class PerformanceInfo
    {
        public List<CPUInfo> Processors { get; set; } = new();
    }

    [Serializable]
    public class CPUInfo
    {
        public List<float> Usage { get; set; } = new();
    }

}
