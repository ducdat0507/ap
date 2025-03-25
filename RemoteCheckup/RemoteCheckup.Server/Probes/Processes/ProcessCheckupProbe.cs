using System.Diagnostics;
using System.ServiceProcess;
using RemoteCheckup.DTOs;

namespace RemoteCheckup.Probes
{
    public abstract class ProcessCheckupProbe
    {
        private Dictionary<int, double> lastCpuTime = new();
        private long lastTimestamp;

        public ProcessesInfo GetProcessesInfo()
        {
            ProcessesInfo info = new();

            GetProcessesInfo(info);
            GetServicesInfo(info);
            GetActivePortsInfo(info);

            return info;
        }

        public abstract void GetServicesInfo(ProcessesInfo info);
        public abstract void GetActivePortsInfo(ProcessesInfo info);

        public void GetProcessesInfo(ProcessesInfo info)
        {
            try
            {
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                long duration = timestamp - lastTimestamp;
                lastTimestamp = timestamp;

                foreach (var proc in Process.GetProcesses())
                {
                    double cpuSec = proc.UserProcessorTime.TotalSeconds;
                    double cpuDelta = lastCpuTime.TryGetValue(proc.Id, out double cpuLast)
                        ? (cpuSec - cpuLast) : 0;
                    info.Processes.Add(new ProcessInfo
                    {
                        PID = proc.Id,
                        Name = proc.ProcessName,
                        CPUUsage = (float)cpuDelta / duration * 1000,
                        MemoryUsage = (ulong)proc.PrivateMemorySize64,
                    });
                    lastCpuTime[proc.Id] = cpuSec;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }
    }
}
