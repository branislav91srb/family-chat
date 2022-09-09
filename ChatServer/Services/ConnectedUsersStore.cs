using Contracts.Models;
using System.Collections.Concurrent;

namespace ChatServer.Services
{
    public class ConnectedUsersStore
    {
        private ConcurrentDictionary<string, ChatUserModel> _connctedUsers = new();

        public List<ChatUserModel> GetUserList()
        {
            return _connctedUsers.Select(x => x.Value).ToList();
        }

        public void AddUser(ChatUserModel user)
        {
            _connctedUsers.TryAdd(user.UserName, user);
        }

        public void RemoveUser(string userName)
        {
            _connctedUsers.TryRemove(userName, out var value);
        }
    }
}
