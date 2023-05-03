using System.Collections.Concurrent;

namespace ChatServer.Hubs
{
    public class OnlineUsersHolder
    {
        // All connected users (connectionId/userId)
        private ConcurrentDictionary<string, string> OnlineUsers = new();

        // TODO: Change UserId (valu from dictionary) to be long
        public List<string> GetAll()
        {
            return OnlineUsers.Select(x => x.Value).ToList();
        }

        public string? GetConnectionIdByUserId(string userId)
        {
            return OnlineUsers.FirstOrDefault(x => x.Value == userId).Key;
        }

        public bool Add(string connectionId, string userId)
        {
            return OnlineUsers.TryAdd(connectionId, userId);
        }

        public string? Remove(string connectionId)
        {
            var removedUser = OnlineUsers.TryRemove(connectionId, out string? userId);

            if (removedUser)
            {
                return userId!;
            }

            return null;
        }
    }
}
