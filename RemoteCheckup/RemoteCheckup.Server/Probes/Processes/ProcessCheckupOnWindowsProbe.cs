using System.Diagnostics;
using System.Runtime.Versioning;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using RemoteCheckup.DTOs;
using RemoteCheckup.Utilities;

namespace RemoteCheckup.Probes
{
    [SupportedOSPlatform("windows")]
    public partial class ProcessCheckupOnWindowsProbe : ProcessCheckupProbe
    {
        public override void GetServicesInfo(ProcessesInfo info)
        {
            var servs = ServiceController.GetServices();
            foreach (var serv in servs)
            {
                info.Services.Add(new ServiceInfo()
                {
                    Name = serv.DisplayName,
                    Status = serv.Status switch
                    {
                        ServiceControllerStatus.Running => ServiceStatus.Running,
                        ServiceControllerStatus.Stopped => ServiceStatus.Inactive,
                        _ => ServiceStatus.Unknown,
                    }
                });
            }
        }
        public override void GetActivePortsInfo(ProcessesInfo info)
        {
            // TODO Implement this
        }
    }
}
