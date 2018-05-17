using System.Collections.Generic;
using System.Linq;
using ChatService.Models;

namespace ChatService.Persistence
{
    internal class ChatMessageMapper
    {
        public ChatMessageEntity Map(ChatMessage chatMessage)
        {
            return new ChatMessageEntity
            {
                TimeStamp = chatMessage.TimeStamp,
                Message = chatMessage.Message,
                Sender = chatMessage.Sender
            };
        }

        public IEnumerable<ChatMessage> Map(IEnumerable<ChatMessageEntity> chatMessages)
        {
            return chatMessages.Select(m => new ChatMessage(){Message = m.Message, TimeStamp = m.TimeStamp, Sender = m.Sender}).ToArray();
        }
    }
}