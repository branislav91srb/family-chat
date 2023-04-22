using ChatServer.Data.Entities;
using ChatServer.Services.Abstraction;
using Contracts;
using Contracts.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatServer.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly IMessageRepository _messageRepository;
        private readonly IConnectedUsersRepository _connectedUsersRepository;


        public ChatHub(ILogger<ChatHub> logger, IMessageRepository messageRepository, IConnectedUsersRepository connectedUsersRepository)
        {
            _logger = logger;
            _messageRepository = messageRepository;
            _connectedUsersRepository = connectedUsersRepository;
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

            await Clients.All.SendAsync("ReceiveMessage", messageData).ConfigureAwait(false);

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
            var chatUser = GetUserDataFromHeaders();

            await UpdateOnlineUsers(chatUser);

            _logger.LogInformation($"{0} entered the room!", chatUser.UserName);

            var messageData = new Message
            {
                Text = $"{chatUser.UserName} entered the room!",
                Sender = new MessageSender
                {
                    UserName = "System",
                    Avatar = chatUser.Avatar
                },
                MessageType = MessageTypeEnum.Notification
            };

            await Clients.All.SendAsync("ReceiveMessage", messageData);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception is not null)
            {
                _logger.LogError(exception.Message, exception);
            }

            var chatUser = GetUserDataFromHeaders();

            await UpdateOnlineUsers(chatUser);

            _logger.LogInformation($"{0} left the room!", chatUser.UserName);

            var messageData = new Message
            {
                Text = $"{chatUser.UserName} left the room!",
                Sender = new MessageSender
                {
                    UserName = "System",
                    Avatar = chatUser.Avatar
                },
                MessageType = MessageTypeEnum.Notification
            };

            await Clients.All.SendAsync("ReceiveMessage", messageData);
            await base.OnDisconnectedAsync(exception);
        }

        private async Task UpdateOnlineUsers(ChatUserModel chatUser, bool remove = false)
        {
            if (remove)
            {
                await _connectedUsersRepository.RemoveUserAsync(chatUser.ConnectionId);
            }
            else
            {
                await _connectedUsersRepository.AddUserAsync(chatUser);
            }
            var allUsers = await _connectedUsersRepository.GetAllUsersAsync().ConfigureAwait(false);

            await Clients.All.SendAsync("UpdateOnlineUsers", allUsers);
        }

        private ChatUserModel GetUserDataFromHeaders()
        {
            var httpContext = Context.GetHttpContext();
            var userName = httpContext?.Request?.Headers["X-USER-NAME"].ToString() ?? "Unknown";
            var userAvatar = httpContext?.Request?.Headers["X-AVATAR"].ToString() ?? "379476_furby.svg";

            return new ChatUserModel
            {
                UserName = userName,
                Avatar = userAvatar,
                ConnectionId = Context.ConnectionId
            };
        }
    }
}
