namespace Contracts
{
    public class Message
    {
        public string Text { get; set; } = string.Empty;

        public string From { get; set; }

        public MessageTypeEnum MessageType { get; set; } = MessageTypeEnum.Message;

        public DateTimeOffset SendTime { get; set; } = DateTimeOffset.Now;
    }
}
