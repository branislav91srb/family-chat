using LocalChatApp.Data.Enitites;

namespace LocalChatApp.Services.Abstraction
{
    public interface IAppSettingsService
    {
        Task SaveAsync(AppSettingsItem appSettingItem);

        Task<List<AppSettingsItem>> GetAllAsync();
    }
}
