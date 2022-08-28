using ChatServer.Data;
using ChatServer.Data.Entities;
using ChatServer.Services.Abstraction;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace ChatServer.Services
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ServerDbContext _db;

        public MessageRepository(ServerDbContext db)
        {
            _db = db;
        }

        public async Task<List<MessageEntity>> GetMessagesAsync(int number = 10)
        {
            var messages = _db.Messages.OrderByDescending(x => x.SendTime).Take(number);

            return await messages.ToListAsync();
        }

        public async Task SaveMessageAsync(MessageEntity message)
        {
            ArgumentNullException.ThrowIfNull(message);

            _db.Messages.Add(message);

            await _db.SaveChangesAsync();
        }
    }
}
