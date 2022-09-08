using LocalChatApp.Data;
using LocalChatApp.Services;
using LocalChatApp.Services.Abstraction;
using LocalChatApp.WinUI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Hosting;

namespace LocalChatApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            builder.Services.AddDbContext<AppDbContext>(opt =>
            {

                var appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                var dbPath = Path.Combine(appData, "localchat.db");
                opt.UseSqlite($"Filename={dbPath}");
            });
            builder.Services.AddLogging();

            //var assembly = Assembly.GetExecutingAssembly();
            //using var stream = assembly.GetManifestResourceStream("LocalChatApp.appsettings.json");

            //var config = new ConfigurationBuilder()
            //            .AddJsonStream(stream)
            //            .Build();


            //builder.Configuration.AddConfiguration(config);


            builder.Services.AddHttpClient();

            builder.Services.AddScoped<IAppSettingsService, AppSettingsService>();
            builder.Services.AddScoped<IServerUriService, ServerUriService>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                Seed.SeedSettings(scope);
            }

            return app;
        }
    }
}