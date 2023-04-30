using ChatServer.Data.Entities;
using ChatServer.Services.Abstraction;
using Contracts.Models;
using Contracts.Requests;

namespace ChatServer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageService _messageService;

        public UserService(IUserRepository userRepository, IMessageService messageRepository)
        {
            _userRepository = userRepository;
            _messageService = messageRepository;
        }

        public async Task<List<UserModel>> GetUsersAsync()
        {
            var users = await _userRepository.GetUsersAsync();

            var result = new List<UserModel>();

            foreach (var user in users)
            {
                result.Add(new UserModel
                {
                    Id = user.Id,
                    Avatar = user.Avatar,
                    UserName = user.UserName
                });
            }

            return result;
        }

        public async Task<List<UserWithLastMessage>> GetUserswithLastMessageAsync(long userForId)
        {
            var users = await GetUsersAsync();
            var lastMessages = await _messageService.GetLastMessagesForUserAsync(userForId);

            var result = new List<UserWithLastMessage>();

            foreach (var user in users)
            {
                var lastMessage = lastMessages.FirstOrDefault(x => x.From == user.Id || x.To == user.Id);
                var userWithMessage = new UserWithLastMessage
                {
                    User = user,
                    Message = lastMessage
                };

                result.Add(userWithMessage);
            }

            return result;
        }

        public async Task<UserEntity> GetUserByIdAsync(long userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        public async Task<long> RegisterUserAsync(RegisterUserRequest user)
        {
            var userEntity = new UserEntity
            {
                UserName = user.UserName,
                Password = user.Password,
                Avatar = user.Avatar
            };
            return await _userRepository.SaveUserAsync(userEntity);
        }

        public async Task<long> SaveUserAsync(SaveUserRequest user)
        {
            if(user.Id == 0)
            {
                throw new ArgumentException(nameof(user.Id));
            }

            var userEntity = new UserEntity
            {
                Id = user.Id,
                UserName = user.UserName,
                Password = user.Password,
                Avatar = user.Avatar
            };
            return await _userRepository.SaveUserAsync(userEntity);
        }

        public async Task<UserEntity?> GetUserByUsernameAndPasswordAsync(string userName, string password)
        {
            ArgumentNullException.ThrowIfNull(userName, nameof(userName));
            ArgumentNullException.ThrowIfNull(password, nameof(password));
            return await _userRepository.GetUserAsync(x => x.UserName == userName && x.Password == password);
        }
    }
}
