using System.Diagnostics;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using RemoteCheckup.DTOs;
using RemoteCheckup.Utilities;

namespace RemoteCheckup.Probes
{
    public partial class ProcessCheckupOnLinuxProbe : ProcessCheckupProbe
    {
        public override void GetServicesInfo(ProcessesInfo info)
        {
            CliUtility.RunAndReadAllLines(
                "/bin/systemctl",
                "list-units -t service --plain --no-legend --no-page",
                (source, args) => {
                    string line = args.Data ?? "";
                    Match match = SystemctlServiceParseRegex().Match(line);
                    if (!match.Success) return;

                    ServiceStatus status = match.Groups["sub"].Value switch 
                    {
                        "running" => ServiceStatus.Running,
                        "exited" => ServiceStatus.Completed,
                        "failed" => ServiceStatus.Faulted,
                        _ => ServiceStatus.Unknown
                    };

                    info.Services.Add(new () {
                        Name = match.Groups["name"].Value,
                        Status = status
                    });
                }
            );
        }

        public override void GetActivePortsInfo(ProcessesInfo info)
        {
            CliUtility.RunAndReadAllLines(
                "ss",
                "-lntupH",
                (source, args) => {
                    string line = args.Data ?? "";
                    Match match = SsPortParseRegex().Match(line);
                    if (!match.Success) return;

                    string? pidString = match.Groups["pid"]?.Value;
                    info.Ports.Add(new () {
                        IsTCP = match.Groups["proto"].Value == "tcp",
                        Status = match.Groups["state"].Value.ToLower() switch {
                            "unconn" => PortStatus.Inactive,
                            "listen" => PortStatus.Listening,
                            _ => PortStatus.Unknown
                        },
                        Port = ushort.Parse(match.Groups["lport"].Value),
                        PID = string.IsNullOrEmpty(pidString) ? null : int.Parse(pidString)
                    });
                }
            );
        }

        [GeneratedRegex(@"^(?<name>.*\.service)\s+(?<load>\S+)\s+(?<active>\S+)\s+(?<sub>\S+)\s+(?<desc>.*?)\s*$")]
        private static partial Regex SystemctlServiceParseRegex();

        [GeneratedRegex(@"^(?<proto>\S+)\s+(?<state>\S+)\s+\S+\s+\S+\s+(?<local>.+):(?<lport>\d+)\s+(?<remote>.*):(?<rport>[\d*]*)\s+(?:users:.*pid=(?<pid>\d+),fd=\d+\)\))?\s*$")]
        private static partial Regex SsPortParseRegex();
    }
}
