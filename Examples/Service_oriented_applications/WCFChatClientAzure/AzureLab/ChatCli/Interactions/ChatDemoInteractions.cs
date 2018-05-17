using ChatService.Interfaces;

namespace ChatCli.Interactions
{
    internal class ChatDemoInteractions
    {
        public static void RunEventLoop(IChatService serviceClient)
        {
            string msg = string.Empty;
            switch (msg)
                {
                    case "q":
                        return;
                    default:
                        ClientConsole.SendMessage(serviceClient, "Message test");
                        ClientConsole.PrintAllMessages(serviceClient);
                        break;
                }
        }
    }
}