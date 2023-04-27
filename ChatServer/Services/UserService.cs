using ChatServer.Data.Entities;
using ChatServer.Services.Abstraction;
using Contracts.Requests;

namespace ChatServer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserEntity>> GetUsersAsync()
        {
            return await _userRepository.GetUsersAsync();
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
