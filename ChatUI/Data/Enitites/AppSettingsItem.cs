using System.ComponentModel.DataAnnotations;

namespace LocalChatApp.Data.Enitites
{
    public class AppSettingsItem
    {
        [Key]
        public AppSettingsItemKeyEnum Id { get; set; }

        public string Key { get => Id.ToString("G"); private set => Id.ToString("G"); }

        public string Value { get; set; }
    }

    public enum AppSettingsItemKeyEnum
    {
        AppServerHost = 1,
        AppServerPort = 2
    }
}
