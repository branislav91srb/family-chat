using System.ComponentModel.DataAnnotations;

namespace ChatServer.Data.Entities
{
    public class MessageEntity
    {
        [Key]
        public long Id { get; set; }

        public long From { get; set; }

        public long To { get; set; }

        public DateTime SendTime { get; set; } = DateTime.Now;

        public string Text { get; set; }
    }
}
