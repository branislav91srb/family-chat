namespace Contracts.Models
{
    public class MessageModel
    {
        public long Id { get; set; }

        public long From { get; set; }

        public long To { get; set; }

        public DateTime SendTime { get; set; }

        public string Text { get; set; }
    }
}
