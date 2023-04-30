using ChatServer.Data.Entities;
using ChatServer.Services.Abstraction;
using Contracts;
using Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace ChatServer.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private const string MessageReceiveEvent = "ReceiveMessage";
        private const string UpdateOnlineUsersEvent = "UpdateOnlineUsers";
        private readonly ILogger<ChatHub> _logger;
        private readonly IMessageRepository _messageRepository;
        private readonly OnlineUsersHolder _onlineUsersHolder;


        public ChatHub(ILogger<ChatHub> logger, IMessageRepository messageRepository, OnlineUsersHolder onlineUsersHolder)
        {
            _logger = logger;
            _messageRepository = messageRepository;
            _onlineUsersHolder = onlineUsersHolder;
        }

        public async Task SendMessage(MessageFromTo fromTo, string message)
        {
            _logger.LogWarning($"{fromTo.From} sent a message to {fromTo.To}.");

            var messageData = new Message
            {
                Text = message,
                Sender = fromTo.From,
                MessageType = MessageTypeEnum.Message
            };

            await Clients.All.SendAsync(MessageReceiveEvent, messageData);

            await _messageRepository.SaveMessageAsync(new MessageEntity
            {
                From = fromTo.From,
                To = fromTo.To,
                SendTime = DateTime.Now,
                Text = message
            });
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
            var connectionId = Context.ConnectionId;

            if(string.IsNullOrWhiteSpace(userId) )
            {
                return;
            }

            _logger.LogInformation($"User {0} entered the room!", userId);

            var addedNewUser = _onlineUsersHolder.Add(connectionId, userId);
            if (addedNewUser)
            {
                await Clients.All.SendAsync(UpdateOnlineUsersEvent, _onlineUsersHolder.GetAll());
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;

            var removedUser = _onlineUsersHolder.Remove(connectionId);

            if (!string.IsNullOrEmpty(removedUser))
            {
                _logger.LogInformation($"{0} left the room!", removedUser);
                await Clients.All.SendAsync(UpdateOnlineUsersEvent, _onlineUsersHolder.GetAll());
            }

            if (exception is not null)
            {
                _logger.LogError(exception.Message, exception);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
