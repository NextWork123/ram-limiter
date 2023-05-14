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

            Process[] processes = Process.GetProcesses();

            while (true)
            {
                foreach (Process process in processes)
                {
                    string processNameLower = process.ProcessName.ToLower();

                    if (processNameLower.Contains("discord") || processNameLower.Contains("chrome") || processNameLower.Contains("firefox") || processNameLower.Contains("edge") || processNameLower.Contains("thorium"))
                    {
                        GC.Collect(); // Force garbage collection
                        GC.WaitForPendingFinalizers(); // Wait for all finalizers to complete before continuing.

                        if (Environment.OSVersion.Platform == PlatformID.Win32NT) // Check OS version platform
                        {
                            SetProcessWorkingSetSize(process.Handle, min, max);
                        }
                    }
                }

                Thread.Sleep(5000); // Pausa di 5 secondi per ridurre l'utilizzo della CPU
            }
        }

        static void Main(string[] args)
        {
            RamLimiter(-1, -1);
        }
    }
}
