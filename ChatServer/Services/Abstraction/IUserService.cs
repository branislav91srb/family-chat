using ChatServer.Data.Entities;
using Contracts.Requests;

namespace ChatServer.Services.Abstraction
{
    public interface IUserService
    {
        Task<List<UserEntity>> GetUsersAsync();
        Task<UserEntity> GetUserByIdAsync(long userId);
        Task<UserEntity?> GetUserByUsernameAndPasswordAsync(string userName, string password);
        Task<long> RegisterUserAsync(RegisterUserRequest user);
        Task<long> SaveUserAsync(SaveUserRequest user);
    }
}
