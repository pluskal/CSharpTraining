using System;
using System.Linq;
using System.ServiceModel;
using ChatCli.Interactions;
using ChatService.Interfaces;
using ChatService.Models;

namespace ChatCli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //In case that you want to connect to locally hosted service.
            //Uri uri = new Uri(string.Format("http://localhost:26007/ChatApi.svc", Environment.MachineName));

            Uri uri = new Uri(string.Format("http://azurelabfit.azurewebsites.net/ChatApi.svc", Environment.MachineName));

            RunChatClient(uri, ChatClientType.Bot);

            WaitForPressedKey();
        }
        private static void RunChatClient(Uri uri, ChatClientType type)
        {
            using (IChatService serviceClient = CreateServiceClient(uri))
            {
                switch (type)
                {
                    case ChatClientType.Demo:
                        ChatDemoInteractions.RunEventLoop(serviceClient);
                        break;
                    case ChatClientType.User:
                        ChatUserInteractions.RunEventLoop(serviceClient);
                        break;
                    case ChatClientType.Bot:
                        ChatBotInteractions.RunEventLoop(serviceClient);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }

        private static void WaitForPressedKey()
        {
            Console.WriteLine("Press ANY key to continue...");
            Console.ReadKey();
        }

        private static IChatService CreateServiceClient(Uri uri)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            EndpointAddress endpoint = new EndpointAddress(uri);
            ChannelFactory<IChatService> channelFactory = new ChannelFactory<IChatService>(binding, endpoint);
            IChatService serviceClient = channelFactory.CreateChannel();
            return serviceClient;
        }
    }
}