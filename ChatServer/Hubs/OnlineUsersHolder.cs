using System.Collections.Concurrent;

namespace ChatServer.Hubs
{
    public class OnlineUsersHolder
    {
        private ConcurrentDictionary<string, string> OnlineUsers = new();

        public List<string> GetAll()
        {
            return OnlineUsers.Select(x => x.Value).ToList();
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
