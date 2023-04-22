namespace Contracts.Models
{
    public class ChatAppModel
    {
        public PlatformsEnum Platform { get; set; }

        public Version CurrentAppVersion { get; set; }

        public string ConnectionId { get; set; }
    }
}
