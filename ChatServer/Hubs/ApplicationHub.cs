using ChatServer.Models;
using ChatServer.Services.Abstraction;
using Contracts.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatServer.Hubs
{
    public class ApplicationHub : Hub
    {
        private readonly ILogger<ApplicationHub> _logger;
        private readonly IConnectedAppsRepository _connectedAppsRepository;

        public ApplicationHub(ILogger<ApplicationHub> logger, IConnectedAppsRepository connectedAppsRepository)
        {
            _logger = logger;
            _connectedAppsRepository = connectedAppsRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var data = GetAppDataFromHeaders();

            if (data.Platform == PlatformsEnum.Android)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, ChatConstansts.Groups.ANDROID_APP);
                _logger.LogInformation($"{Context.ConnectionId} added to {ChatConstansts.Groups.ANDROID_APP} group.");
            }
            else if (data.Platform == PlatformsEnum.WinUI)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, ChatConstansts.Groups.WINROWS_APP);
                _logger.LogInformation($"{Context.ConnectionId} added to {ChatConstansts.Groups.WINROWS_APP} group.");
            }

            await _connectedAppsRepository.AddAppAsync(data);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception is not null)
            {
                _logger.LogError(exception, $"{Context.ConnectionId} dropped out.");
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatConstansts.Groups.ANDROID_APP);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatConstansts.Groups.WINROWS_APP);

            await _connectedAppsRepository.RemoveAppAsync(Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        private ChatAppModel GetAppDataFromHeaders()
        {
            var httpContext = Context.GetHttpContext();
            var platformHeader = httpContext?.Request?.Headers["X-PLATFORM"].ToString();
            var appVersionHeader = httpContext?.Request?.Headers["X-APP-VERSION"].ToString();

            if (!Enum.TryParse(platformHeader, true, out PlatformsEnum platform))
            {
                throw new Exception("Platform not valid");
            }

            Version? version = default;
            if (!string.IsNullOrEmpty(appVersionHeader))
            {
                version = new Version(appVersionHeader);
            }

            return new ChatAppModel
            {
                Platform = platform,
                CurrentAppVersion = version ?? new Version(),
                ConnectionId = Context.ConnectionId
            };
        }
    }
}
