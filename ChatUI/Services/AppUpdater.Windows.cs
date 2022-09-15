using FluentFTP;
using LocalChatApp.Data.Enitites;
using LocalChatApp.Services.Abstraction;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO.Compression;

namespace LocalChatApp.Services
{
    public class AppUpdater : IAppUpdater
    {
        private const int FtpPort = 21;

        private readonly IAppSettingsService _appSettingsService;
        private readonly ILogger<AppUpdater> _logger;

        public AppUpdater(IAppSettingsService appSettingsService, ILogger<AppUpdater> logger)
        {
            _appSettingsService = appSettingsService;
            _logger = logger;
        }


        public async Task<string> DownloadUpdate(CancellationToken cancellationToken = default)
        {
            var serverUri = (await _appSettingsService.GetAsync(AppSettingsItemKeyEnum.UpdateUrl)).Value;

            using FtpClient ftpClient = new FtpClient(serverUri, FtpPort, "*****", "*****");
            IList<FtpListItem> items = await ftpClient.GetListingAsync("/home/branislav91/local-chat/windows-app", FtpListOption.ForceList);
            IList<string> listStr = items.Where(x => x.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)).Select(itm => itm.Input).ToList();

            var latestUpdateFile = items.OrderBy(x => x.Name).FirstOrDefault();

            if (latestUpdateFile is null)
            {
                return null;
            }

            var fileBytes = await ftpClient.DownloadAsync(latestUpdateFile.FullName, cancellationToken);

            var cachepath = FileSystem.Current.AppDataDirectory;
            var filePath = Path.Combine(cachepath, latestUpdateFile.Name);

            await File.WriteAllBytesAsync(filePath, fileBytes, cancellationToken);

            return filePath;
        }

        public async Task<bool> Update(string filePath)
        {
            await Task.Delay(3000);

            var filePathParts = filePath.Split("\\");
            var destinationDirectory = string.Join("\\", filePathParts.Take(filePathParts.Length - 1));
            var unzipedDirectory = filePath.Replace(".zip", "");

            if (Directory.Exists(unzipedDirectory))
            {
                Directory.Delete(unzipedDirectory, true);
            }

            ZipFile.ExtractToDirectory(filePath, destinationDirectory);

            var msixFile  = Directory.GetFiles(unzipedDirectory, "*.msix").FirstOrDefault();

            if(msixFile is null)
            {
                return false;
            }

            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = msixFile,
            };

            try
            {
                using Process exeProcess = Process.Start(startInfo);
                exeProcess?.WaitForExit();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Windows app update failed.");
                return false;
            }

            return true;
        }
    }
}
