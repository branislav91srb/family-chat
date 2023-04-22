using ChatServer.Services.Abstraction;
using Contracts.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ChatServer.Services
{
    public class ConnectedUsersRepository : IConnectedUsersRepository
    {
        public const string CACHE_KEY = "ConnectedUsers";
        private readonly IMemoryCache _memoryCache;

        public ConnectedUsersRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task AddUserAsync(ChatUserModel user)
        {
            var allUsers = await GetAllUsersAsync();

            allUsers[user.ConnectionId] = user;

            _memoryCache.Set(CACHE_KEY, allUsers, DateTimeOffset.Now.AddDays(1));
        }

        public Task<Dictionary<string, ChatUserModel>> GetAllUsersAsync()
        {
            var allUsers = (Dictionary<string, ChatUserModel>)_memoryCache.Get(CACHE_KEY);
            allUsers ??= new Dictionary<string, ChatUserModel>();

            return Task.FromResult(allUsers);
        }

        public async Task<ChatUserModel> GetUserAsync(string connectionId)
        {
            var allUsers = await GetAllUsersAsync();

            return allUsers[connectionId];
        }

        public async Task RemoveUserAsync(string connectionId)
        {
            var allUsers = await GetAllUsersAsync();

            allUsers.Remove(connectionId);

            _memoryCache.Set(CACHE_KEY, allUsers, DateTimeOffset.Now.AddDays(1));
        }
    }
}
