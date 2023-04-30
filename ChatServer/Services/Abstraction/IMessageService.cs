using Contracts.Models;
using Contracts.Responses;

namespace ChatServer.Services.Abstraction
{
    public interface IMessageService
    {
        Task<MessagesResponse> GetDirectMessagesAsync(long user1, long user2, int number);
        Task<List<MessageModel>> GetLastMessagesForUserAsync(long userId);
    }
}
