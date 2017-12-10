using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Processes
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var processes = new List<Process>();
            for (var n = 0; n < 10; n++)
            {
                var process = RunProcess(n.ToString());
                processes.Add(process);
            }

            foreach (var process in processes)
            {
                try
                {
                    process?.WaitForExit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
               
            }
            
            WaitForPressedKey();
        }

        private static Process RunProcess(string args)
        {
            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = "ProcessChild.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = args
            };
            try
            {
                return Process.Start(startInfo);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private static void WaitForPressedKey()
        {
            Console.WriteLine("Press any key to exit . . . ");
            Console.ReadKey();
        }
    }
}