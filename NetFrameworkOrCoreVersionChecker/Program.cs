using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace NetFrameworkOrCoreVersionChecker
{
    public class Program
    {
        const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
        public static void Main(string[] args)
        {            
            NetCoreVersion(".net core runtime", "dotnet --list-runtimes");
            NetCoreVersion(".net core sdks", "dotnet --list-sdks");
            NetFrameworkVersion(".net framework version");
            Console.ReadKey();
        }

        private static void NetFrameworkVersion(string caption)
        {
            SetColourCaption(caption);
            using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {

                    Console.WriteLine($"{CheckFor45PlusVersion((int)ndpKey.GetValue("Release"))}");
                }
                else
                {
                    Console.WriteLine(".NET Framework Version 4.5 or later is not detected.");
                }
            }
        }

        private static void NetCoreVersion(string caption, string script)
        {
            try
            {
                PowerShell powerShell;
                powerShell = PowerShell.Create().AddScript(script);
                SetColourCaption(caption);
                foreach (dynamic item in powerShell.Invoke().ToList())
                {
                    Console.WriteLine(item);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("net core not found.");
            }           
        }


        // Checking the version using >= enables forward compatibility.
        static string CheckFor45PlusVersion(int releaseKey)
        {
            if (releaseKey >= 528040)
                return "4.8 or later";
            if (releaseKey >= 461808)
                return "4.7.2";
            if (releaseKey >= 461308)
                return "4.7.1";
            if (releaseKey >= 460798)
                return "4.7";
            if (releaseKey >= 394802)
                return "4.6.2";
            if (releaseKey >= 394254)
                return "4.6.1";
            if (releaseKey >= 393295)
                return "4.6";
            if (releaseKey >= 379893)
                return "4.5.2";
            if (releaseKey >= 378675)
                return "4.5.1";
            if (releaseKey >= 378389)
                return "4.5";
            // This code should never execute. A non-null release key should mean
            // that 4.5 or later is installed.
            return "No 4.5 or later version detected";
        }


        private static void SetColourCaption(string caption)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(caption);
            Console.ResetColor();
        }

    }
}
