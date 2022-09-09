using LocalChatApp.Services.Abstraction;
using Microsoft.AspNetCore.SignalR.Client;

namespace LocalChatApp.Services
{
    public class ChatHubConnectionFactory : IChatHubConnectionFactory
    {
        public const string ChatHubName = "chatHub";

        private readonly IServerUriService _serverUriService;

        public ChatHubConnectionFactory(IServerUriService serverUriService)
        {
            _serverUriService = serverUriService;
        }

        public async Task<HubConnection> CreateConnectionAsync(Dictionary<string, string> headers = null)
        {
            var serverUri = await _serverUriService.GetServerUriAsync();
            var chatHubAddres = $"{serverUri}/{ChatHubName}";

            var connection = new HubConnectionBuilder()
            .WithUrl(chatHubAddres, options =>
            {
                headers?.ToList().ForEach(x =>
                {
                    options.Headers.Add(x.Key, x.Value);
                });
            })
            .Build();

            return connection;
        }
    }
}
