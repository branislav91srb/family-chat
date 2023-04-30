using Contracts.Models;

namespace Contracts.Responses
{
    public class GetUsersResponse
    {
        public List<UserWithLastMessage> UsersWithLastMessage { get; set; } = new();
    }
}
