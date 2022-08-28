namespace LocalChatApp.Services.Abstraction
{
    public interface IServerUriService
    {
        Task<string> GetServerUriAsync();
    }
}
