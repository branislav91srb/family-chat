using ChatServer.Data.Entities;
using ChatServer.Services.Abstraction;
using Contracts;
using Contracts.Models;
using Contracts.Responses;

namespace ChatServer.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repository;

        public MessageService(IMessageRepository repository)
        {
            _repository = repository;
        }

        public async Task<MessagesResponse> GetDirectMessagesAsync(long user1, long user2, int number)
        {
            var messages = await _repository.GetDirectMessagesAsync(user1, user2, number);

            var messagesResponse = new MessagesResponse();

            foreach (var message in messages.OrderBy(x => x.SendTime))
            {
                var messageresponse = new Message
                {
                    SendTime = message.SendTime,
                    Text = message.Text,
                    Sender = message.From
                };

                messagesResponse.Messages.Add(messageresponse);
            }

            return messagesResponse;
        }

        public async Task<List<MessageModel>> GetLastMessagesForUserAsync(long userId)
        {
            var messages = await _repository.GetLastMessagesForUserAsync(userId);

            return messages.Select(x => new MessageModel
            {
                From = x.From,
                To = x.To,
                Id = x.Id,
                SendTime = x.SendTime,
                Text = x.Text
            }).ToList();
        }
    }
}
