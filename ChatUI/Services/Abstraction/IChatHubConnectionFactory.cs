using Microsoft.AspNetCore.SignalR.Client;

namespace LocalChatApp.Services.Abstraction
{
    public interface IChatHubConnectionFactory
    {
        Task<HubConnection> CreateConnectionAsync(Dictionary<string, string> headers = null);
    }
}
