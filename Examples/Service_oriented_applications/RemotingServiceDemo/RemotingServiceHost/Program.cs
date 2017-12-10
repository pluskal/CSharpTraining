using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace RemotingServiceHost
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var remotingService = new HelloRemotingService.HelloRemotingService();
            var channel = new TcpChannel(8081);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(HelloRemotingService.HelloRemotingService),
                "GetMessage",
                WellKnownObjectMode.Singleton);
            Console.WriteLine("Host started @ " + DateTime.Now);
            Console.WriteLine("Press ANY key to continue...");
            Console.ReadKey();
        }
    }
}