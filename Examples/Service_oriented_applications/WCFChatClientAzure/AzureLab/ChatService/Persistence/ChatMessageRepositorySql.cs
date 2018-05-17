using System.Collections.Generic;
using System.Data.Entity;
using ChatService.Interfaces;
using ChatService.Models;

namespace ChatService.Persistence
{
    public class ChatMessageRepositorySql : IChatMessageRepository
    {
        private readonly ChatMessageMapper _chatMessageMapper = new ChatMessageMapper();
        private readonly ChatDbContext _chatDbContext;
        
        public ChatMessageRepositorySql(ChatDbContext chatDbContext)
        {
            this._chatDbContext = chatDbContext;
        }

        public void AddMessage(ChatMessage chatMessage)
        {
            this._chatDbContext.ChatMessages.Add(this._chatMessageMapper.Map(chatMessage));
            this._chatDbContext.SaveChanges();
        }

        public IEnumerable<ChatMessage> GetAllMessages()
        {
                return this._chatMessageMapper.Map(this._chatDbContext.ChatMessages);
        }

        public void ClearMessages()
        {
            this._chatDbContext.DeleteAll<ChatMessageEntity>();
            this._chatDbContext.SaveChanges();
        }

        public void Dispose()
        {
            this._chatDbContext?.Dispose();
        }
    }
}