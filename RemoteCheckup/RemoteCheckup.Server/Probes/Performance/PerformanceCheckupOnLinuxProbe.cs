
using Microsoft.AspNetCore.Http.Features;
using RemoteCheckup.DTOs;
using RemoteCheckup.Utilities;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace RemoteCheckup.Probes
{
    [SupportedOSPlatform("linux")]
    public partial class PerformanceCheckupOnLinuxProbe : PerformanceCheckupProbe
    {
        private float ClockSpeed = 0;
        private Dictionary<string, ulong> lastCpuTime = new();
        private long lastCpuTimestamp = 0;

        Dictionary<string, ulong> lastBytesRead { get; set; } = new();
        Dictionary<string, ulong> lastBytesWritten { get; set; } = new();
        long lastDriveTimestamp { get; set; } = 0;

        public PerformanceCheckupOnLinuxProbe()
        {
            
            using var process = CliUtility.Run(
                "/bin/getconf",
                "CLK_TCK"
            );
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
                        memoryInfo.TotalPhys = kilobytes * 1000;
                        break;
                    }
                    case "MemAvailable": {
                        ulong.TryParse(match.Groups[2].Value, out ulong kilobytes);
                        memoryInfo.UsedPhys = memoryInfo.TotalPhys - kilobytes * 1000;
                        break;
                    }
                    case "MemFree": {
                        ulong.TryParse(match.Groups[2].Value, out ulong kilobytes);
                        memoryInfo.CommittedPhys = memoryInfo.TotalPhys - kilobytes * 1000;
                        break;
                    }
                    case "SwapTotal": {
                        ulong.TryParse(match.Groups[2].Value, out ulong kilobytes);
                        memoryInfo.TotalSwap = kilobytes * 1000;
                        break;
                    }
                    case "SwapFree": {
                        ulong.TryParse(match.Groups[2].Value, out ulong kilobytes);
                        memoryInfo.UsedSwap = memoryInfo.TotalSwap - kilobytes * 1000;
                        break;
                    }
                }
            }
        }

        public override void GetDriveInfo(PerformanceInfo info)
        {
            using Process p = CliUtility.Run(
                "/bin/lsblk",
                @"-b -o name,label,type,rota,model,log-sec,fssize,fsused,mountpoints --json"
            );
            var lsblkOutput = JsonSerializer.DeserializeAsync<LsblkOutput>(p.StandardOutput.BaseStream);
            
            Dictionary<string, ulong> sectorsRead = [];
            Dictionary<string, ulong> sectorsWritten = [];
            foreach (string line in File.ReadAllLines("/proc/diskstats"))
            {
                string[] items = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                sectorsRead[items[2]] = Convert.ToUInt64(items[5]);
                sectorsWritten[items[2]] = Convert.ToUInt64(items[9]);
            }
 
            p.WaitForExit();
            try { lsblkOutput.AsTask().Wait(); }
            catch (Exception e) { Console.WriteLine(e); return; }

            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long duration = timestamp - lastDriveTimestamp;
            lastDriveTimestamp = timestamp;

            if (lsblkOutput.IsCompletedSuccessfully) 
            {
                LsblkOutput lsblkResult = lsblkOutput.Result!;
                foreach (var block in lsblkResult.BlockDevices) 
                {
                    if (block.MountPoints?.Contains("[SWAP]") != false) continue;
                    if (block.BlockType != "disk") continue;

                    ulong rd = sectorsRead.GetValueOrDefault(block.Name);
                    ulong wr = sectorsWritten.GetValueOrDefault(block.Name);
                    ulong rdDelta = lastBytesRead.TryGetValue(block.Name, out ulong rdLast)
                        ? (rd - rdLast) : 0;
                    ulong wrDelta = lastBytesWritten.TryGetValue(block.Name, out ulong wrLast)
                        ? (wr - wrLast) : 0;

                    lastBytesRead[block.Name] = rd;
                    lastBytesWritten[block.Name] = wr;

                    DTOs.DriveInfo driveInfo = new () {
                        Name = block.DriveModel,
                        IsHDD = block.IsHDD,
                        ReadSpeed = (ulong)((double)rdDelta * block.SectorSize / duration * 1000),
                        WriteSpeed = (ulong)((double)wrDelta * block.SectorSize / duration * 1000),
                    };
                    info.Drives.Add(driveInfo);

                    foreach (var part in block.Children) 
                    {
                        if (part.MountPoints?.Count <= 0 || part.MountPoints?.Any(x => x == "[SWAP]" || x == null) != false) continue;
                        if (part.BlockType != "part") continue;
                        if ((Convert.ToUInt64(part.PartitionFlags) & 0x8000_0000_0000_0000) != 0) continue;
                        driveInfo.Partitions.Add(new PartitionInfo() {
                            Name = part.Name,
                            TotalBytes = Convert.ToUInt64(part.FileSystemSize),
                            UsedBytes = Convert.ToUInt64(part.FileSystemUsed),
                        });
                    }
                }
            }
        }

        [GeneratedRegex(@"^(\w+):\s+(\d+) ?kB")]
        private static partial Regex MeminfoParseRegex();

        [Serializable]
        class LsblkOutput
        {
            [JsonPropertyName("blockdevices")]
            public List<LsblkBlock> BlockDevices { get; set; } = new();
        }

        [Serializable]
        class LsblkBlock
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = "";
            [JsonPropertyName("label")]
            public string Label { get; set; } = "";
            [JsonPropertyName("partflags")]
            public string PartitionFlags { get; set; } = "0";
            [JsonPropertyName("rota")]
            public bool IsHDD { get; set; } = false;
            [JsonPropertyName("model")]
            public string DriveModel { get; set; } = "";
            [JsonPropertyName("fssize")]
            public string FileSystemSize { get; set; } = "";
            [JsonPropertyName("fsused")]
            public string FileSystemUsed { get; set; } = "";
            [JsonPropertyName("type")]
            public string BlockType { get; set; } = "";
            [JsonPropertyName("log-sec")]
            public ulong SectorSize { get; set; } = 0;
            [JsonPropertyName("mountpoints")]
            public List<string> MountPoints { get; set; } = [];
            [JsonPropertyName("children")]
            public List<LsblkBlock> Children { get; set; } = [];
        }
    }
}
