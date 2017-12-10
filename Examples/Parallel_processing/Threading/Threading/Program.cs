using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Threading
{
    class Program
    {
        private static readonly Random Random = new Random();
        static void Main(string[] args)
        {
            PrintBasicInfo();
           
            var explicitThread = RunExplicitThread();

            RunThreadPoolThreads();

            SayHelloDelegate asyncAdd = SayHello;
            var asyncResult = asyncAdd.BeginInvoke(11, null, null);
            
            SleepRandom();

            WaitForPressedKey();

            asyncResult.AsyncWaitHandle.WaitOne();
            explicitThread.Join();
        }

        private static void WaitForPressedKey()
        {
            Console.WriteLine("Press any key to exit . . . ");
            Console.ReadKey();
        }

        /// <summary>
        /// ThreadPool queued threads cannot be explicitly Joined.
        /// </summary>
        private static void RunThreadPoolThreads()
        {
            for (int n = 0; n < 10; n++)
            {
                ThreadPool.QueueUserWorkItem(SayHello, n);
            }
        }

        private static Thread RunExplicitThread()
        {
            Thread explicitThread = new Thread(SayHello)
            {
                Name = "Hello Thread",
                Priority = ThreadPriority.BelowNormal,
                IsBackground = true
            };
            explicitThread.Start(10);
            return explicitThread;
        }

        private static void PrintBasicInfo()
        {
            Console.WriteLine("Multi Threading Demo");
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}], Main called");

            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Processor/core count = {Environment.ProcessorCount}");
            Console.WriteLine($"Is 64 Bit OS = {Environment.Is64BitOperatingSystem}");
        }

        delegate void SayHelloDelegate(object index);
        static void SayHello(object index)
        {
            SleepRandom();
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Hello, World {index}! ({Thread.CurrentThread.IsBackground})");
        }

        private static void SleepRandom()
        {
            Thread.Sleep(Random.Next(250, 500));
        }
    }
}
