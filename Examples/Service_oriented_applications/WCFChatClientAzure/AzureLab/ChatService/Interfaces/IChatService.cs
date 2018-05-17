using System;
using System.Collections.Generic;
using System.ServiceModel;
using ChatService.Models;

namespace ChatService.Interfaces
{
    [ServiceContract]
    public interface IChatService : IDisposable
    {
        [OperationContract]
        void SendMessage(ChatMessage chatMessage);

        [OperationContract]
        void ClearMessages();

        [OperationContract]
        IEnumerable<ChatMessage> GetAllMessages();
    }
}