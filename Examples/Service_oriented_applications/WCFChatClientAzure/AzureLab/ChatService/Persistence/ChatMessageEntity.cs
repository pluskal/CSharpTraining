using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatService.Persistence
{
    public class ChatMessageEntity
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public DateTime TimeStamp { get; set; }
        public string Sender { get; set; }
        public string Message { get; set; }
    }
}