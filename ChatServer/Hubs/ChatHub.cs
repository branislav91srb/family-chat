using ChatServer.Data.Entities;
using ChatServer.Services.Abstraction;
using Contracts;
using Microsoft.AspNetCore.SignalR;

namespace ChatServer.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly IMessageRepository _messageRepository;

        public ChatHub(ILogger<ChatHub> logger, IMessageRepository messageRepository)
        {
            _logger = logger;
            _messageRepository = messageRepository;
        }

        public async Task SendMessage(MessageSender sender, string message)
        {
            _logger.LogWarning($"{sender.UserName} sent a message.");

            var messageData = new Message
            {
                Text = message,
                Sender = sender,
                MessageType = MessageTypeEnum.Message
            };

            await Clients.All.SendAsync("ReceiveMessage", messageData);

            await _messageRepository.SaveMessageAsync(new MessageEntity
            {
                From = sender.UserName,
                To = "All",
                SendTime = DateTime.Now,
                Text = message
            });
        }

        public override async Task OnConnectedAsync()
        {
            var user = Context.UserIdentifier;

            _logger.LogInformation($"{0} entered the room!", user);

            var messageData = new Message
            {
                Text = $"{user} entered the room!",
                Sender = new MessageSender
                {
                    UserName = "System",
                    Avatar = "379476_furby.svg"
                },
                MessageType = MessageTypeEnum.Notification
            };

            await Clients.All.SendAsync("ReceiveMessage", messageData);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = Context.UserIdentifier;

            if(exception != null)
            {
                _logger.LogError(exception.Message, exception);
            }

            _logger.LogInformation($"{0} left the room!", user);

            var messageData = new Message
            {
                Text = $"{user} left the room!",
                Sender = new MessageSender
                {
                    UserName = "System",
                    Avatar = "379476_furby.svg"
                },
                MessageType = MessageTypeEnum.Notification
            };

            await Clients.All.SendAsync("ReceiveMessage", messageData);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
