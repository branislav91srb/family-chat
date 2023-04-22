using ChatServer.Services.Abstraction;
using Contracts.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ChatServer.Services
{
    public class ConnectedAppsRepository : IConnectedAppsRepository
    {
        public const string CACHE_KEY = "ConnectedApps";
        private readonly IMemoryCache _memoryCache;

        public ConnectedAppsRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task<Dictionary<string, ChatAppModel>> GetAllAppsAsync()
        {
            var allApps = (Dictionary<string, ChatAppModel>)_memoryCache.Get(CACHE_KEY);
            allApps ??= new Dictionary<string, ChatAppModel>();

            return Task.FromResult(allApps);
        }

        public async Task<ChatAppModel> GetAppAsync(string connectionId)
        {
            var allApps = await GetAllAppsAsync();

            return allApps[connectionId];
        }

        public async Task AddAppAsync(ChatAppModel app)
        {
            var allApps = await GetAllAppsAsync();

            allApps[app.ConnectionId] = app;

            _memoryCache.Set(CACHE_KEY, allApps, DateTimeOffset.Now.AddDays(1));
        }

        public async Task RemoveAppAsync(string connectionId)
        {
            var allApps = await GetAllAppsAsync();

            allApps.Remove(connectionId);

            _memoryCache.Set(CACHE_KEY, allApps, DateTimeOffset.Now.AddDays(1));
        }

    }
}
