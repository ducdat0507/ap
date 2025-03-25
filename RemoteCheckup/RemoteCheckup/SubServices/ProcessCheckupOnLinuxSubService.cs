using System.Diagnostics;
using System.Runtime.Versioning;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using RemoteCheckup.DTOs;
using RemoteCheckup.Utilities;

namespace RemoteCheckup.SubServices
{
    [SupportedOSPlatform("linux")]
    public partial class ProcessCheckupOnLinuxSubService : ProcessCheckupSubService
    {
        public override void GetServicesInfo(ProcessesInfo info)
        {
            CliUtility.RunAndReadAllLines(
                "/bin/systemctl",
                "list-units -t service --plain --no-legend --no-page",
                (source, args) => {
                    string line = args.Data ?? "";
                    Match match = ServiceParseRegex().Match(line);
                    if (!match.Success) return;

                    ServiceStatus status = match.Groups[4].Value switch 
                    {
                        "running" => ServiceStatus.Running,
                        "exited" => ServiceStatus.Completed,
                        "failed" => ServiceStatus.Faulted,
                        _ => ServiceStatus.Unknown
                    };

                    info.Services.Add(new () {
                        Name = match.Groups[1].Value,
                        Status = status
                    });
                }
            );
        }

        [GeneratedRegex(@"^(.*\.service)\s+(\S+)\s+(\S+)\s+(\S+)\s+(.*)$")]
        private static partial Regex ServiceParseRegex();
    }
}
