using ChatServer.Requests;

namespace ChatServer.Services.Abstraction
{
    public interface IMessageService
    {
        Task SendMessageAsync(SendMessageRequest message);
    }
}
