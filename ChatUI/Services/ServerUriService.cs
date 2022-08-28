using LocalChatApp.Data.Enitites;
using LocalChatApp.Services.Abstraction;

namespace LocalChatApp.Services
{
    public class ServerUriService : IServerUriService
    {
        private readonly IAppSettingsService _appSettingsService;

        public ServerUriService(IAppSettingsService appSettingsService)
        {
            _appSettingsService = appSettingsService;
        }

        public async Task<string> GetServerUriAsync()
        {
            var allSettings = await _appSettingsService.GetAllAsync();

            var host = allSettings.Single(x => x.Id == AppSettingsItemKeyEnum.AppServerHost).Value;
            var port = allSettings.Single(x => x.Id == AppSettingsItemKeyEnum.AppServerPort).Value;

            return $"{host}:{port}";
        }
    }
}
