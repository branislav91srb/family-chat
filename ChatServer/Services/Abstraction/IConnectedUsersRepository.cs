using Contracts.Models;

namespace ChatServer.Services.Abstraction
{
    public interface IConnectedUsersRepository
    {
        Task<ChatUserModel> GetUserAsync(string connectionId);

        Task<Dictionary<string, ChatUserModel>> GetAllUsersAsync();

        Task AddUserAsync(ChatUserModel user);

        Task RemoveUserAsync(string connectionId);
    }
}
