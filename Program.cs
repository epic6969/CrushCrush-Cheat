using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using Serilog;
using Serilog.Core;
using System.Threading;

namespace CrushCrush_Cheat
{
    internal static class Program
    {
        private static bool RanAsAdmin => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        private static Logger log = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        private static void Cheat()
        {
            Retry:
            if (Process.GetProcesses().All(p => p.ProcessName != "CrushCrush"))
            {
                log.Error("Couldn't find CrushCrush process! Retrying..");
                Thread.Sleep(2500);
                goto Retry;
            }
            var proc = Process.GetProcessesByName("CrushCrush")[0];
            var hProc = GHApi.OpenProcess(GHApi.ProcessAccessFlags.All, false, proc.Id);
            log.Information($"Opened Process {proc.ProcessName}");
            var modBase = GHApi.GetModuleBaseAddress(proc.Id, "CrushCrush.exe");
            var gemAddr = GHApi.FindDMAAddy(hProc, modBase + 0x01037CE8, new[] {0xb4, 0x68, 0x30, 0x24, 0x8, 0xc});
            log.Information("How many gems do you want?");
            var newGems = int.Parse(Console.ReadLine() ?? "9999");
            var updatedGems = GHApi.WriteProcessMemory(hProc, gemAddr, newGems, 4, out _);
            log.Information($"Success: {updatedGems}");
            Console.ReadKey();
        }
        
        private static void Main(string[] args)
        {
            Console.Title = "CrushCrush Cheat || Made by ePiC";
            if (!RanAsAdmin)
            {
                log.Fatal(new Exception("Run Application as Administrator"), "Please run CrushCrush Cheat as administrator!");
                Thread.Sleep(1250);
                Process.GetCurrentProcess().Kill();
            }
            Cheat();
        }
    }
}