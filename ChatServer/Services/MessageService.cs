using ChatServer.Data;
using ChatServer.Data.Entities;
using ChatServer.Hubs;
using ChatServer.Requests;
using ChatServer.Services.Abstraction;

namespace ChatServer.Services
{
    public class MessageService : IMessageService
    {
        private readonly ServerDbContext _db;

        public MessageService(ServerDbContext db, ChatHub chatHub)
        {
            _db = db;
        }

        public async Task SendMessageAsync(SendMessageRequest message)
        {
            _db.Messages.Add(new MessageEntity
            {
                From = message.From,
                To = message.To,
                Text = message.Message,
                SendTime = DateTime.Now
            });

            await _db.SaveChangesAsync();
        }
    }
}
