using RemoteCheckup.Models;
using System.Diagnostics;
using System.Management;
using System.Runtime.Versioning;

namespace RemoteCheckup.SubServices
{
    [SupportedOSPlatform("windows")]
    public class PerformanceCheckupOnWindowsSubService : PerformanceCheckupSubService
    {
        private ManagementObjectSearcher cpuSearcher;

        private Dictionary<string, float> lastCpuTime = new();
        private Dictionary<string, float> lastCpuTimestamp = new();

        public PerformanceCheckupOnWindowsSubService()
        {
            cpuSearcher = new("select * from Win32_PerfRawData_PerfOS_Processor");
        }

        public override PerformanceInfo GetPerformanceInfo()
        {
            PerformanceInfo info = new();

            var cpuTimes = cpuSearcher.Get()
                .Cast<ManagementObject>()
                .Select(mo => new
                {
                    Name = (string)mo["Name"],
                    Index = (string)mo["Name"] == "_Total" ? -1 : Convert.ToInt32(mo["Name"]),
                    Usage = (float)(ulong)mo["PercentProcessorTime"] / (ulong)mo["Frequency_PerfTime"],
                    Timestamp = (float)(ulong)mo["Timestamp_PerfTime"] / (ulong)mo["Frequency_PerfTime"],
                }
                )
                .ToList();

            info.Processors.Add(new()
            {
                Usage = cpuTimes.Where(x => x.Index >= 0)
                    .OrderBy(x => x.Index)
                    .Select(x => {
                        float actualTime = lastCpuTime.TryGetValue(x.Name, out float lastTime)
                            ? (x.Usage - lastTime) : 0;
                        float duration = lastCpuTime.TryGetValue(x.Name, out float lastTimestamp)
                            ? (x.Timestamp - lastTimestamp) : 0;
                        actualTime = 1 - actualTime / duration;
                        lastCpuTime[x.Name] = x.Usage;
                        return actualTime;
                    }).ToList(),
            });

            return info;
        }
    }
}
