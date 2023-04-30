namespace Contracts.Models
{
    public class UserWithLastMessage
    {
        public UserModel User { get; set; }

        public MessageModel? Message { get; set; }
    }
}
