using System;
using System.Collections.Generic;
using ChatService.Models;

namespace ChatService.Interfaces
{
    public interface IChatMessageRepository: IDisposable
    {
        void AddMessage(ChatMessage chatMessage);
        IEnumerable<ChatMessage> GetAllMessages();
        void ClearMessages();
    }
}