using Contracts.Models;

namespace ChatServer.Services.Abstraction
{
    public interface IConnectedAppsRepository
    {
        Task<ChatAppModel> GetAppAsync(string connectionId);

        Task<Dictionary<string, ChatAppModel>> GetAllAppsAsync();

        Task AddAppAsync(ChatAppModel app);

        Task RemoveAppAsync(string connectionId);
    }
}
