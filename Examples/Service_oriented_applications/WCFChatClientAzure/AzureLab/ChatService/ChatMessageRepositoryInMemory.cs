using System.Collections.Generic;
using ChatService.Interfaces;
using ChatService.Models;

namespace ChatService
{
    public class ChatMessageRepositoryInMemory : IChatMessageRepository
    {
        private static List<ChatMessage> Repository { get; } = new List<ChatMessage>();

        public void AddMessage(ChatMessage chatMessage)
        {
            Repository.Add(chatMessage);
        }

        public IEnumerable<ChatMessage> GetAllMessages()
        {
            return Repository;
        }

        public void ClearMessages()
        {
            Repository.Clear();
        }

        public void Dispose()
        {
        }
    }
}