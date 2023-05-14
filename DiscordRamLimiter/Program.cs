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
            int discordId = -1;
            long workingSet = 0;
            foreach (Process discord in Process.GetProcessesByName("Discord"))
            {
                if (discord.WorkingSet64 > workingSet)
                {
                    workingSet = discord.WorkingSet64;
                    discordId = discord.Id;
                }
            }
            return discordId;
        }

        public static int GetThorium()
        {
            int thoriumId = -1;
            long workingSet = 0;
            foreach (Process thorium in Process.GetProcessesByName("Thorium"))
            {
                if (thorium.WorkingSet64 > workingSet)
                {
                    workingSet = thorium.WorkingSet64;
                    thoriumId = thorium.Id;
                }
            }
            return thoriumId;
        }

        static void RamLimiter(int min, int max)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            while (GetDiscord() != -1 || GetThorium() != -1)
            {
                if (GetDiscord() != -1)
                {
                    GC.Collect(); // Force garbage collection
                    GC.WaitForPendingFinalizers(); // Wait for all finalizers to complete before continuing.

                    if (Environment.OSVersion.Platform == PlatformID.Win32NT) // Check OS version platform
                    {
                        SetProcessWorkingSetSize(Process.GetProcessById(GetDiscord()).Handle, min, max);
                    }

                    var wmiObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
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
                        Console.ForegroundColor = ConsoleColor.Green;
                        Thread.Sleep(600);
                    }

                    Thread.Sleep(1);
                }

                if (GetThorium() != -1)
                {
                    GC.Collect(); // Force garbage collection
                    GC.WaitForPendingFinalizers(); // Wait for all finalizers to complete before continuing.

                    if (Environment.OSVersion.Platform == PlatformID.Win32NT) // Check OS version platform
                    {
                        SetProcessWorkingSetSize(Process.GetProcessById(GetThorium()).Handle, min, max);
                    }

                    var wmiObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
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
                        Console.ForegroundColor = ConsoleColor.Green;
                        Thread.Sleep(600);
                    }

                    Thread.Sleep(1);
                }
            }
        }

        static void Main(string[] args)
        {
            Console.Write("Discord and Thorium RAM Limiter - ");
            RamLimiter(-1, -1);
        }
    }
}

