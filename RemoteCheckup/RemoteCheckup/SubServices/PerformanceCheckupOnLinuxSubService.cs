
using RemoteCheckup.Models;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace RemoteCheckup.SubServices
{
    [SupportedOSPlatform("linux")]
    public partial class PerformanceCheckupOnLinuxSubService : PerformanceCheckupSubService
    {
        private float ClockSpeed = 0;
        private Dictionary<string, ulong> lastCpuTime = new();
        private long lastCpuTimestamp = 0;

        public PerformanceCheckupOnLinuxSubService()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/getconf",
                Arguments = "CLK_TCK",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            process.WaitForExit();
            float.TryParse(process.StandardOutput.ReadToEnd(), out ClockSpeed);
        }

        public override void GetCPUInfo(PerformanceInfo info) 
        {
            string[] stat = File.ReadAllLines("/proc/stat");
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            float ticks = (timestamp - lastCpuTimestamp) * ClockSpeed / 1000;
            lastCpuTimestamp = timestamp;

            foreach (var line in stat) 
            {
                string[] items = line.Split(" ");
                if (items.Length < 0) continue;
                if (items[0] == "cpu")
                {
                    CPUInfo cpuInfo = new ();
                    info.Processors.Add(cpuInfo);
                }
                else if (items[0].StartsWith("cpu")) 
                {
                    ulong.TryParse(items[4], out ulong cpuTime);
                    ulong idleTime = lastCpuTime.TryGetValue(items[0], out ulong lastTime)
                        ? (cpuTime - lastTime) : 0;
                    float actualUsage = 1 - idleTime / ticks;
                    info.Processors[^1].Usage.Add(actualUsage);
                    info.Processors[^1].TotalUsage += actualUsage;
                    lastCpuTime[items[0]] = cpuTime;
                }
            }

            info.Processors[^1].TotalUsage /= info.Processors[^1].Usage.Count;
        }

        public override void GetMemoryInfo(PerformanceInfo info) 
        {
            string[] meminfo = File.ReadAllLines("/proc/meminfo");
            MemoryInfo memoryInfo = info.Memory = new();

            foreach (var line in meminfo) 
            {
                var match = MeminfoParseRegex().Match(line);
                if (match == null) return;
                switch (match.Groups[1].Value)
                {
                    case "MemTotal": {
                        ulong.TryParse(match.Groups[2].Value, out ulong kilobytes);
                        memoryInfo.TotalBytes = kilobytes * 1000;
                        break;
                    }
                    case "MemAvailable": {
                        ulong.TryParse(match.Groups[2].Value, out ulong kilobytes);
                        memoryInfo.UsedBytes = memoryInfo.TotalBytes - kilobytes * 1000;
                        break;
                    }
                    case "MemFree": {
                        ulong.TryParse(match.Groups[2].Value, out ulong kilobytes);
                        memoryInfo.CommittedBytes = memoryInfo.TotalBytes - kilobytes * 1000;
                        break;
                    }
                }
            }
        }

        [GeneratedRegex(@"^(\w+):\s+(\d+) ?kB")]
        private static partial Regex MeminfoParseRegex();
    }
}
