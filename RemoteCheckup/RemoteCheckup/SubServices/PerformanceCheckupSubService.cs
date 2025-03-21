using RemoteCheckup.Models;

namespace RemoteCheckup.SubServices
{
    public abstract class PerformanceCheckupSubService
    {
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
        public abstract void GetNetworkInfo(PerformanceInfo info);
    }
}
