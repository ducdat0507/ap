using Microsoft.AspNetCore.SignalR;

namespace RemoteCheckup.Models
{
    [Serializable]
    public class PerformanceInfo
    {
        public class CPUInfo 
        {
            public List<double> Usage = new();
        }

        public List<CPUInfo> Processors = new();
    }

}
