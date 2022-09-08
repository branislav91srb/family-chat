using System.ComponentModel.DataAnnotations;

namespace LocalChatApp.Data.Enitites
{
    public class AppSettingsItemEntity
    {
        [Key]
        public AppSettingsItemKeyEnum Id { get; set; }

        public string Key { get => Id.ToString("G"); private set => Id.ToString("G"); }

        public string Value { get; set; }
    }

    public enum AppSettingsItemKeyEnum
    {
        AppServerHost = 1,
        AppServerPort = 2,
        DefaultAvatar = 3
    }
}
