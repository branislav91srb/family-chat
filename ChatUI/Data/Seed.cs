using LocalChatApp.Data.Enitites;
using Microsoft.AspNetCore.Hosting;
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

            var settings = new List<AppSettingsItemEntity> {
                new AppSettingsItemEntity { Id = AppSettingsItemKeyEnum.AppServerHost, Value="http://localhost" },
                new AppSettingsItemEntity { Id = AppSettingsItemKeyEnum.AppServerPort, Value="9001"},
                new AppSettingsItemEntity { Id = AppSettingsItemKeyEnum.AppServerPort, Value="9001"},
            };

            context.AppSettings.AddRange(settings);
            context.SaveChanges();
        }
    }
}
