using System;
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

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static void RamLimiter(int min, int max)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            string[] targetProcessNames = { "discord", "chrome", "firefox", "edge", "thorium" };

            while (true)
            {
                Process[] processes = Process.GetProcesses()
                    .Where(p => targetProcessNames.Any(name => p.ProcessName.ToLower().Contains(name)))
                    .ToArray();

                foreach (Process process in processes)
                {
                    if (!process.HasExited && process.Responding && Environment.OSVersion.Platform == PlatformID.Win32NT)
                    {
                        SetProcessWorkingSetSize(process.Handle, min, max);
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