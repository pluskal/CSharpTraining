using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks
{
    internal class Program
    {
        private static readonly Random Random = new Random();

        private static void Main(string[] args)
        {
            var tasks = new List<Task>();
            for (var i = 0; i < 10; i++)
                tasks.Add(SayHello(i));

            Task.WhenAll(tasks).Wait();

            WaitForPressedKey();
        }

        private static async Task SayHello(int index)
        {
            //await Task.Delay();
            await Task.Delay(await GetRandom());
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Hello, World {index}! ({Thread.CurrentThread.IsBackground})");
        }

        private static async Task<int> GetRandom()
        {
          return await Task.Run(() => Random.Next(250, 500));
        }

        private static void WaitForPressedKey()
        {
            Console.WriteLine("Press any key to exit . . . ");
            Console.ReadKey();
        }
    }
}