using System.ComponentModel.DataAnnotations;

namespace ChatServer.Requests
{
    public class SendMessageRequest
    {
        [Required]
        public string From { get; set; }

        [Required]
        public string To { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
