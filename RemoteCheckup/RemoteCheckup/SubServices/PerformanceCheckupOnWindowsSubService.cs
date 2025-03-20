
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

        private Dictionary<string, ulong> lastCpuTime = new();
        private Dictionary<string, ulong> lastCpuTimestamp = new();

        public PerformanceCheckupOnWindowsSubService()
        {
            cpuSearcher = new("select * from Win32_PerfRawData_PerfOS_Processor");
        }

        public override void GetCPUInfo(PerformanceInfo info) 
        {
            var cpuTimes = cpuSearcher.Get()
                .Cast<ManagementObject>()
                .Select(mo => new
                {
                    Name = (string)mo["Name"],
                    Index = (string)mo["Name"] == "_Total" ? -1 : Convert.ToInt32(mo["Name"]),
                    Usage = (ulong)mo["PercentProcessorTime"],
                    Timestamp = (ulong)mo["Timestamp_PerfTime"],
                }
                )
                .ToList();

            float totalUsage = 0;

            info.Processors.Add(new()
            {
                Usage = cpuTimes.Where(x => x.Index >= 0)
                    .OrderBy(x => x.Index)
                    .Select(x => {
                        ulong idleTime = lastCpuTime.TryGetValue(x.Name, out ulong lastTime)
                            ? (x.Usage - lastTime) : 0;
                        ulong duration = lastCpuTimestamp.TryGetValue(x.Name, out ulong lastTimestamp)
                            ? (x.Timestamp - lastTimestamp) : 0;
                        float actualTime = Math.Max(1 - (float)idleTime / duration, 0);
                        if (!float.IsFinite(idleTime)) idleTime = 0;
                        lastCpuTime[x.Name] = x.Usage;
                        lastCpuTimestamp[x.Name] = x.Timestamp;
                        totalUsage += actualTime;
                        return actualTime;
                    }).ToList(),
            });
            info.Processors[^1].TotalUsage = totalUsage / info.Processors[^1].Usage.Count();
        }

        public override void GetMemoryInfo(PerformanceInfo info) 
        {
            // TODO Implement this
        }
    }
}
