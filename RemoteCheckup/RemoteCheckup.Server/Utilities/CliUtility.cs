using System.Diagnostics;

namespace RemoteCheckup.Utilities
{
    static class CliUtility 
    {
        static public Process Run(string exe, string args) 
        {
            Process p = new();
            p.StartInfo.FileName = exe;
            p.StartInfo.Arguments = args;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            return p;
        }

        static public void RunAndReadAllLines(string exe, string args, DataReceivedEventHandler onLineRead) 
        {
            Process p = new();
            p.StartInfo.FileName = exe;
            p.StartInfo.Arguments = args;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.OutputDataReceived += onLineRead;
            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();
        }
    }
}