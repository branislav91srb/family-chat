using LocalChatApp.Data.Enitites;

namespace LocalChatApp.Services.Abstraction
{
    public interface IAppSettingsService
    {
        Task SaveAsync(AppSettingsItemEntity appSettingItem);

        Task<List<AppSettingsItemEntity>> GetAllAsync();

        Task<AppSettingsItemEntity> GetAsync(AppSettingsItemKeyEnum id);
    }
}
