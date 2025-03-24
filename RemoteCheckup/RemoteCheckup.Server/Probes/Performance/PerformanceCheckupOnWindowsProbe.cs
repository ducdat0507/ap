
using RemoteCheckup.DTOs;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace RemoteCheckup.Probes
{
    [SupportedOSPlatform("windows")]
    public class PerformanceCheckupOnWindowsProbe : PerformanceCheckupProbe
    {
        private ManagementObjectSearcher cpuSearcher;

        private Dictionary<string, ulong> lastCpuTime = new();
        private Dictionary<string, ulong> lastCpuTimestamp = new();

        public PerformanceCheckupOnWindowsProbe()
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
            info.Processors[^1].TotalUsage = totalUsage / info.Processors[^1].Usage.Count;
        }

        public override void GetMemoryInfo(PerformanceInfo info)
        {
            MEMORYSTATUSEX memStatus = new();
            GlobalMemoryStatusEx(memStatus);
            info.Memory = new() {
                TotalPhys = memStatus.ullTotalPhys,
                UsedPhys = memStatus.ullTotalPhys - memStatus.ullAvailPhys,
            };
            info.Memory.TotalSwap = memStatus.ullTotalPageFile - info.Memory.TotalPhys;
            info.Memory.UsedSwap = memStatus.ullTotalPageFile - memStatus.ullAvailPageFile - info.Memory.UsedPhys;
        }

        public override void GetDriveInfo(PerformanceInfo info)
        {
            // TODO Implement this
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx([In] [Out] MEMORYSTATUSEX lpBuffer);
    }
}
