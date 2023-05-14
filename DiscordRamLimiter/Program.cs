using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace DiscordRamLimiter
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

        static string[] targetProcessNames = { "discord", "chrome", "firefox", "edge", "thorium" };

        static void RamLimiter(int min, int max)
        {
            bool isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
            List<Process> processBuffer = new List<Process>();

            while (true)
            {
                if (isWindows)
                {
                    processBuffer.Clear();
                    processBuffer.AddRange(Process.GetProcesses()
                        .Where(process =>
                            targetProcessNames.Any(name => process.ProcessName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0) &&
                            !process.HasExited && process.Responding));

                    foreach (Process process in processBuffer)
                    {
                        if (!process.HasExited)
                        {
                            SetProcessWorkingSetSize(process.Handle, min, max);
                        }
                    }
                }

                Thread.Sleep(1000); // Pause for 1 second to reduce CPU usage
            }
        }

        static void Main(string[] args)
        {
            RamLimiter(-1, -1);
        }
    }
}
