using ChatServer.Data;
using ChatServer.Data.Entities;
using ChatServer.Services.Abstraction;
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

        public async Task<List<MessageEntity>> GetDirectMessagesAsync(long user1, long user2, int number = 10)
        {
            var messages = _db.Messages
                .Where(x => (x.From == user1 && x.To == user2)
                || (x.From == user2 && x.To == user1))
                .OrderByDescending(x => x.SendTime).Take(number);

            return await messages.ToListAsync();
        }

        public async Task SaveMessageAsync(MessageEntity message)
        {
            ArgumentNullException.ThrowIfNull(message);

            _db.Messages.Add(message);

            await _db.SaveChangesAsync();
        }

        public async Task<List<MessageEntity>> GetLastMessagesForUserAsync(long userId)
        {

            var messages = await _db.Messages.FromSql(@$"
                SELECT * FROM Messages,
                  (
                    SELECT MAX(id) as lastid FROM Messages
                    WHERE
                        Messages.[To] = {userId}
                        OR Messages.[From] = {userId}
                    GROUP BY
                      (
                        MIN(Messages.[To], messages.[From])
                        || '.'
                        || MAX(Messages.[To], messages.[From])
                      )
                  ) as conversations
                WHERE
                  Id = conversations.lastid
                ORDER BY
                  Messages.SendTime DESC
             ").ToListAsync();

            return messages;
        }
    }
}
