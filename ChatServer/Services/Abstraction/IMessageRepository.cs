using ChatServer.Data.Entities;

namespace ChatServer.Services.Abstraction
{
    public interface IMessageRepository
    {
        Task<List<MessageEntity>> GetDirectMessagesAsync(long user1, long user2, int number = 10);

        Task SaveMessageAsync(MessageEntity message);
    }
}
