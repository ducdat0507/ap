using System.Net.NetworkInformation;
using RemoteCheckup.DTOs;

namespace RemoteCheckup.Probes
{
    public abstract class PerformanceCheckupProbe
    {
        private Dictionary<string, ulong> lastBytesSent { get; set; } = new();
        private Dictionary<string, ulong> lastBytesReceived { get; set; } = new();
        private long lastNetworkTimestamp { get; set; }

        public PerformanceInfo GetPerformanceInfo()
        {
            PerformanceInfo info = new();

            GetCPUInfo(info);
            GetMemoryInfo(info);
            GetDriveInfo(info);
            GetNetworkInfo(info);

            return info;
        }

        public abstract void GetCPUInfo(PerformanceInfo info);
        public abstract void GetMemoryInfo(PerformanceInfo info);
        public abstract void GetDriveInfo(PerformanceInfo info);

        public void GetNetworkInfo(PerformanceInfo info)
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long duration = timestamp - lastNetworkTimestamp;
            lastNetworkTimestamp = timestamp;

            foreach (NetworkInterface ni in nics)
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
                var stats = ni.GetIPStatistics();
                ulong upl = (ulong)stats.BytesSent;
                ulong dwl = (ulong)stats.BytesReceived;
                ulong uplDelta = lastBytesSent.TryGetValue(ni.Name, out ulong uplLast)
                    ? (upl - uplLast) : 0;
                ulong dwlDelta = lastBytesReceived.TryGetValue(ni.Name, out ulong dwlLast)
                    ? (dwl - dwlLast) : 0;

                lastBytesSent[ni.Name] = upl;
                lastBytesReceived[ni.Name] = dwl;

                var netinfo = new NetworkInfo()
                {
                    Name = ni.Name,
                    Type = ni.NetworkInterfaceType,
                    DownloadSpeed = (ulong)((float)dwlDelta * 1000 / duration),
                    UploadSpeed = (ulong)((float)uplDelta * 1000 / duration),
                };
                info.Networks.Add(netinfo);
            }
        }

    }
}
