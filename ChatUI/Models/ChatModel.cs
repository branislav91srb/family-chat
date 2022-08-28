using Contracts;

namespace LocalChatApp.Models
{
    public class ChatModel
    {
        public string Message { get; set; } = string.Empty;

        public List<Message> Messages { get; set; } = new();
    }
}
