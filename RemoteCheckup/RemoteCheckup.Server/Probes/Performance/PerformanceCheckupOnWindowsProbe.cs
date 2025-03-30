
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
        private ManagementObjectSearcher driveSearcher;

        private Dictionary<string, ulong> lastCpuTime = new();
        private Dictionary<string, ulong> lastCpuTimestamp = new();

        private Dictionary<string, ulong> lastDriveRead = new();
        private Dictionary<string, ulong> lastDriveWrite = new();
        private Dictionary<string, ulong> lastDriveTimestamp = new();

        public PerformanceCheckupOnWindowsProbe()
        {
            cpuSearcher = new("select * from Win32_PerfRawData_PerfOS_Processor");
            driveSearcher = new("select * from Win32_PerfRawData_PerfDisk_PhysicalDisk");
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
            var driveTimes = driveSearcher.Get()
                .Cast<ManagementObject>()
                .Select(mo => new
                {
                    Name = (string)mo["Name"],
                    Index = (string)mo["Name"] == "_Total" ? -1 : Convert.ToInt32(((string)mo["Name"]).Split(" ")[0]),
                    ReadBytes = (ulong)mo["DiskReadBytesPersec"],
                    WriteBytes = (ulong)mo["DiskWriteBytesPersec"],
                    Timestamp = (ulong)mo["Timestamp_PerfTime"],
                }
                )
                .ToList();

            var volumes = System.IO.DriveInfo.GetDrives();

            foreach (var dt in driveTimes)
            {
                if (dt.Index < 0) continue;

                ulong readDelta = lastDriveRead.TryGetValue(dt.Name, out ulong lastReadBytes)
                    ? (dt.ReadBytes - lastReadBytes) : 0;
                ulong writeDelta = lastDriveWrite.TryGetValue(dt.Name, out ulong lastWriteBytes)
                    ? (dt.WriteBytes - lastWriteBytes) : 0;
                ulong duration = lastDriveTimestamp.TryGetValue(dt.Name, out ulong lastTimestamp)
                    ? (dt.Timestamp - lastTimestamp) : 0;

                info.Drives.Add(new DTOs.DriveInfo()
                {
                    Name = dt.Name,
                    IsHDD = null,
                    ReadSpeed = (ulong)(readDelta / (float)duration * TimeSpan.TicksPerSecond),
                    WriteSpeed = (ulong)(writeDelta / (float)duration * TimeSpan.TicksPerSecond),
                });

                lastDriveRead[dt.Name] = dt.ReadBytes;
                lastDriveWrite[dt.Name] = dt.WriteBytes;
                lastDriveTimestamp[dt.Name] = dt.Timestamp;
            }

            foreach (var vol in volumes)
            {
                int index = info.Drives.FindIndex(x => x.Name.Contains(vol.Name[0] + ":"));
                if (index < 0) index = info.Drives.Count - 1;
                try
                {
                    info.Drives[index].Partitions.Add(new PartitionInfo()
                    {
                        Name = $"{vol.Name} {vol.VolumeLabel}",
                        TotalBytes = (ulong)vol.TotalSize,
                        UsedBytes = (ulong)(vol.TotalSize - vol.AvailableFreeSpace)
                    });
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Retrieving drive {vol.Name} info failed: \n - {e}");
                }
            }
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
