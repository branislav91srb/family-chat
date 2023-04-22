using ChatServer.Hubs;
using ChatServer.Models;
using ChatServer.Services.Abstraction;
using Contracts.Models;
using FluentFTP;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace ChatServer.Services
{
    public class UpdateChacker : BackgroundService
    {
        private readonly TimeSpan _period = TimeSpan.FromSeconds(30);
        private List<string> _notifiedApps = new();

        private readonly ILogger<UpdateChacker> _logger;
        private readonly FtpSettings _ftpSettings;
        private readonly IHubContext<ApplicationHub> _hubContext;
        private readonly IConnectedAppsRepository _connectedAppsRepository;

        public UpdateChacker(ILogger<UpdateChacker> logger,
            IOptions<FtpSettings> ftpSettings,
            IHubContext<ApplicationHub> hubContext,
            IConnectedAppsRepository connectedAppsRepository)
        {
            _logger = logger;
            _ftpSettings = ftpSettings.Value;
            _hubContext = hubContext;
            _connectedAppsRepository = connectedAppsRepository;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_period);

            while (!stoppingToken.IsCancellationRequested &&
                await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await UpdateChackAndNotification();
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Failed to execute update check on {_ftpSettings.Host}.");
                    _logger.LogError(ex, ex.Message);
                }
            }
        }

        private async Task UpdateChackAndNotification()
        {
            using var ftpClient = new FtpClient(_ftpSettings.Host, _ftpSettings.UserName, _ftpSettings.Password, _ftpSettings.Port);

            var (androidUpdate, androidAppVersion) = GetAndroidUpdate(ftpClient);
            var (windowsUpdate, windowsAppVersion) = GetWindowsUpdate(ftpClient);

            var allAppsConnected = await _connectedAppsRepository.GetAllAppsAsync();

            _logger.LogInformation($"Latest app versions are: {androidAppVersion} for Android, and {windowsAppVersion} for Windows.");

            if (androidUpdate is not null && allAppsConnected.Any(x => x.Value.Platform == PlatformsEnum.Android))
            {
                var usersAlreadyUpToDate = allAppsConnected
                    .Where(x => x.Value.Platform == PlatformsEnum.Android && x.Value.CurrentAppVersion.Equals(androidAppVersion))
                    .Select(x => x.Key)
                    .ToList();

                await SendNotification(ChatConstansts.Groups.ANDROID_APP, usersAlreadyUpToDate);
            }

            if (windowsUpdate is not null && allAppsConnected.Any(x => x.Value.Platform == PlatformsEnum.WinUI))
            {
                var usersAlreadyUpToDate = allAppsConnected
                    .Where(x => x.Value.Platform == PlatformsEnum.WinUI && x.Value.CurrentAppVersion.Equals(windowsAppVersion))
                    .Select(x => x.Key)
                    .ToList();

                await SendNotification(ChatConstansts.Groups.WINROWS_APP, usersAlreadyUpToDate);
            }
        }

        private async Task SendNotification(string group, List<string> expectUsers)
        {
            var allAppsConnected = await _connectedAppsRepository.GetAllAppsAsync();

            if (!allAppsConnected.Any())
            {
                return;
            }

            expectUsers.AddRange(_notifiedApps);

            if (!allAppsConnected.Any(x => !expectUsers.Contains(x.Key)))
            {
                return;
            }

            await _hubContext.Clients.GroupExcept(group, expectUsers).SendAsync("UpdateNotification", "New update available!");

            var notifiedApps = allAppsConnected.Keys.ToList();
            notifiedApps.RemoveAll(x => expectUsers.Contains(x));

            _notifiedApps.AddRange(notifiedApps);
        }

        private (FtpListItem? File, Version? Version) GetAndroidUpdate(FtpClient ftpClient)
        {
            var updateFile = ftpClient.GetListing($"{_ftpSettings.Location}/android-app", FtpListOption.ForceList)
                .Where(x => x.Name.EndsWith(".apk"))
                .OrderByDescending(x => x.Name)
                .FirstOrDefault();

            if (updateFile is null)
            {
                return (null, null);
            }

            var version = GetFileVersion(updateFile.Name);

            return (updateFile, version);
        }

        private (FtpListItem? File, Version? Version) GetWindowsUpdate(FtpClient ftpClient)
        {
            var updateFile = ftpClient.GetListing($"{_ftpSettings.Location}/windows-app", FtpListOption.ForceList)
                .Where(x => x.Name.EndsWith(".msix"))
                .OrderByDescending(x => x.Name)
                .FirstOrDefault();

            if (updateFile is null)
            {
                return (null, null);
            }

            var version = GetFileVersion(updateFile.Name);

            return (updateFile, version);
        }

        private Version GetFileVersion(string fileName)
        {
            var versionPart = string.Join('.', fileName.Split('-').Last().Split('.')[..^1]);

            if (!Version.TryParse(versionPart, out Version? version))
            {
                return new Version();
            }

            return version;
        }

    }
}
