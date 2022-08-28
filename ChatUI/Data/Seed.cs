using LocalChatApp.Data.Enitites;
using Microsoft.EntityFrameworkCore;

namespace LocalChatApp.Data
{
    public class Seed
    {
        public static void SeedSettings(IServiceScope serviceScope)
        {
            using var context = new AppDbContext(serviceScope.ServiceProvider.GetRequiredService<DbContextOptions<AppDbContext>>());

            var alreadyExists = context.AppSettings.Any();

            if (alreadyExists)
                return;

            var settings = new List<AppSettingsItem> {
                new AppSettingsItem { Id = AppSettingsItemKeyEnum.AppServerHost, Value="http://localhost" },
                new AppSettingsItem { Id = AppSettingsItemKeyEnum.AppServerPort, Value="9001"}
            };

            context.AppSettings.AddRange(settings);
            context.SaveChanges();
        }

    }
}
