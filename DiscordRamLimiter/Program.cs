using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
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

        public static int GetDiscord()
        {
            Process[] discordProcesses = Process.GetProcessesByName("Discord");
            if (discordProcesses.Length > 0)
            {
                Process discord = discordProcesses.OrderByDescending(p => p.WorkingSet64).FirstOrDefault();
                return discord.Id;
            }
            return -1;
        }

        public static int GetThorium()
        {
            Process[] thoriumProcesses = Process.GetProcessesByName("Thorium");
            if (thoriumProcesses.Length > 0)
            {
                Process thorium = thoriumProcesses.OrderByDescending(p => p.WorkingSet64).FirstOrDefault();
                return thorium.Id;
            }
            return -1;
        }

        static void RamLimiter(int min, int max)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            ManagementObjectSearcher wmiObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

            while (true)
            {
                int discordId = GetDiscord();
                int thoriumId = GetThorium();

                if (discordId != -1)
                {
                    GC.Collect(); // Force garbage collection
                    GC.WaitForPendingFinalizers(); // Wait for all finalizers to complete before continuing.

                    if (Environment.OSVersion.Platform == PlatformID.Win32NT) // Check OS version platform
                    {
                        SetProcessWorkingSetSize(Process.GetProcessById(discordId).Handle, min, max);
                    }

                    var memoryValues = wmiObject.Get().Cast<ManagementObject>().Select(mo => new
                    {
                        FreePhysicalMemory = Double.Parse(mo["FreePhysicalMemory"].ToString()),
                        TotalVisibleMemorySize = Double.Parse(mo["TotalVisibleMemorySize"].ToString())
                    }).FirstOrDefault();

                    if (memoryValues != null)
                    {
                        var percent = ((memoryValues.TotalVisibleMemorySize - memoryValues.FreePhysicalMemory) / memoryValues.TotalVisibleMemorySize) * 100;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Your current memory usage (Discord): {0}", percent);
                        Console.ResetColor();
                    }
                    Thread.Sleep(600);
                }

                if (thoriumId != -1)
                {
                    GC.Collect(); // Force garbage collection
                    GC.WaitForPendingFinalizers(); // Wait for all finalizers to complete before continuing.

                    if (Environment.OSVersion.Platform == PlatformID.Win32NT) // Check OS version platform
                    {
                        SetProcessWorkingSetSize(Process.GetProcessById(thoriumId).Handle, min, max);
                    }

                    var memoryValues = wmiObject.Get().Cast<ManagementObject>().Select(mo => new
                    {
                        FreePhysicalMemory = Double.Parse(mo["FreePhysicalMemory"].ToString()),
                        TotalVisibleMemorySize = Double.Parse(mo["TotalVisibleMemorySize"].ToString())
                    }).FirstOrDefault();

                    if (memoryValues != null)
                    {
                        var percent = ((memoryValues.TotalVisibleMemorySize - memoryValues.FreePhysicalMemory) / memoryValues.TotalVisibleMemorySize) * 100;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Your current memory usage (Thorium): {0}", percent);
                        Console.ResetColor();
                    }

                    Thread.Sleep(600);
                }

                Thread.Sleep(1);
            }
        }

        static void Main(string[] args)
        {
            Console.Write("Discord and Thorium RAM Limiter - ");
            RamLimiter(-1, -1);
        }
    }
}
