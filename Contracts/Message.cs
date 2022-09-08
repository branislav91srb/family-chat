namespace Contracts
{
    public class Message
    {
        public string Text { get; set; } = string.Empty;

        public MessageSender Sender { get; set; } = new();

        public MessageTypeEnum MessageType { get; set; } = MessageTypeEnum.Message;

        public DateTimeOffset SendTime { get; set; } = DateTimeOffset.Now;
    }

    public class MessageSender
    {
        public string UserName { get; set; } = string.Empty;

        public string Avatar { get; set; } = string.Empty;
    }
}
