using RemoteCheckup.Models;
using System.Net.NetworkInformation;

namespace RemoteCheckup.SubServices
{
    public interface IPerformanceGlobalNetworkCheckupSubService
    {
        protected Dictionary<string, long> lastBytesSent { get; set; }
        protected Dictionary<string, long> lastBytesReceived { get; set; }
        protected long lastNetworkTimestamp { get; set; }

        public void GetNetworkInfoGlobal(PerformanceInfo info)
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long duration = timestamp - lastNetworkTimestamp;
            lastNetworkTimestamp = timestamp;

            foreach (NetworkInterface ni in nics)
            {
                var stats = ni.GetIPStatistics();
                long upl = stats.BytesSent;
                long dwl = stats.BytesReceived;
                long uplDelta = lastBytesSent.TryGetValue(ni.Name, out long uplLast)
                    ? (upl - uplLast) : 0;
                long dwlDelta = lastBytesReceived.TryGetValue(ni.Name, out long dwlLast)
                    ? (dwl - dwlLast) : 0;

                Console.WriteLine(uplDelta, dwlDelta);
                lastBytesSent[ni.Name] = upl;
                lastBytesReceived[ni.Name] = dwl;

                var netinfo = new NetworkInfo()
                {
                    Name = ni.Name,
                    Type = (int)ni.NetworkInterfaceType,
                    DownloadSpeed = (ulong)((float)dwlDelta * 1000 / duration),
                    UploadSpeed = (ulong)((float)uplDelta * 1000 / duration),
                };
                info.Networks.Add(netinfo);
            }
        }

    }
}
