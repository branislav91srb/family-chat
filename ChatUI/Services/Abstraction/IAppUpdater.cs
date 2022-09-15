namespace LocalChatApp.Services.Abstraction
{
    public interface IAppUpdater
    {
        Task<string> DownloadUpdate(CancellationToken cancellationToken = default);

        Task<bool> Update(string filePath);
    }
}
