using FluentFTP;
using LocalChatApp.Services.Abstraction;
using Android.Content;
using AndroidApp = Android.App.Application;
using AndroidUri = Android.Net.Uri;

namespace LocalChatApp.Services
{
    public class AppUpdater : IAppUpdater
    {
        private Context context = AndroidApp.Context;

        private const int FtpPort = 21;

        private readonly IAppSettingsService _appSettingsService;

        public AppUpdater(IAppSettingsService appSettingsService)
        {
            _appSettingsService = appSettingsService;
        }

        public async Task<string> DownloadUpdate(CancellationToken cancellationToken = default)
        {
            var serverUri = (await _appSettingsService.GetAsync(AppSettingsItemKeyEnum.UpdateUrl)).Value;

            using FtpClient ftpClient = new FtpClient(serverUri, FtpPort, "*****", "*****");
            IList<FtpListItem> items = await ftpClient.GetListingAsync("/home/branislav91/local-chat/android-app", FtpListOption.ForceList);
            IList<string> listStr = items.Select(itm => itm.Input).ToList();

            var latestUpdateFile = items.OrderBy(x => x.Name).FirstOrDefault();

            if (latestUpdateFile is null)
            {
                return null;
            }

            var fileBytes = await ftpClient.DownloadAsync(latestUpdateFile.FullName, cancellationToken);

            var androidCachepath = AndroidApp.Context.CacheDir.Path;

            var filePath = Path.Combine(androidCachepath, latestUpdateFile.Name);

            await File.WriteAllBytesAsync(filePath, fileBytes, cancellationToken);

            return filePath;
        }

        public async Task<bool> Update(string filePath)
        {
            await Task.Delay(3000);

            var file = new Java.IO.File(filePath);

            AndroidUri path = AndroidX.Core.Content.FileProvider.GetUriForFile(context, context.PackageName + ".fileprovider", file);

            var promptInstall = new Intent(Intent.ActionView)
                .SetDataAndType(path, "application/vnd.android.package-archive");

            promptInstall.AddFlags(ActivityFlags.NewTask);
            promptInstall.AddFlags(ActivityFlags.GrantReadUriPermission);

            context.StartActivity(promptInstall);

            return true;
        }
    }

}
