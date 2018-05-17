

using System.Data.Entity;

namespace ChatService.Persistence
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(string connectionString) : base(connectionString)
        {
            Database.SetInitializer<ChatDbContext>(new DropCreateDatabaseIfModelChanges<ChatDbContext>());
        }
        
        public DbSet<ChatMessageEntity> ChatMessages { get; set; }
    }
}
