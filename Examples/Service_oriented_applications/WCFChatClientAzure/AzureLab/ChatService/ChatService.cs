using System;
using System.Collections.Generic;
using ChatService.Interfaces;
using ChatService.Models;
using ChatService.Persistence;

namespace ChatService
{
    public class ChatService : IChatService
    {
        /// <summary>
        /// For the sake of simplicity, there is no DI
        /// For an example with DI, look at https://stackoverflow.com/questions/2454850/how-do-i-pass-values-to-the-constructor-on-my-wcf-service/2455039#2455039
        /// </summary>
        public ChatService()
        {
            //this.ChatMessageRepository = new ChatMessageRepositoryInMemory();
            this.ChatMessageRepository = new ChatMessageRepositorySql(new ChatDbContext("azurelab"));
        }

        public ChatService(IChatMessageRepository chatMessageRepository)
        {
            this.ChatMessageRepository = chatMessageRepository;
        }

        private IChatMessageRepository ChatMessageRepository { get; }

        public void SendMessage(ChatMessage chatMessage)
        {
            if (chatMessage == null)
                throw new ArgumentNullException(nameof(chatMessage));
            if (chatMessage.TimeStamp == DateTime.MinValue) chatMessage.TimeStamp = DateTime.Now;

            this.ChatMessageRepository.AddMessage(chatMessage);
        }

        public void ClearMessages()
        {
            this.ChatMessageRepository.ClearMessages();
        }

        public IEnumerable<ChatMessage> GetAllMessages()
        {
            return this.ChatMessageRepository.GetAllMessages();
        }

        public void Dispose()
        {
            this.ChatMessageRepository?.Dispose();
        }
    }
}