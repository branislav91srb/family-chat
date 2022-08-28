using System.ComponentModel.DataAnnotations;

namespace ChatServer.Data.Entities
{
    public class MessageEntity
    {
        [Key]
        public long Id { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public DateTime SendTime { get; set; }

        public string Text { get; set; }
    }
}
