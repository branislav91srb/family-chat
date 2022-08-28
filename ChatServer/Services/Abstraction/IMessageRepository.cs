using ChatServer.Data.Entities;

namespace ChatServer.Services.Abstraction
{
    public interface IMessageRepository
    {
        Task<List<MessageEntity>> GetMessagesAsync(int number = 10);

        Task SaveMessageAsync(MessageEntity message);
    }
}
