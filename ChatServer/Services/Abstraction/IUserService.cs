using ChatServer.Data.Entities;
using Contracts.Models;
using Contracts.Requests;

namespace ChatServer.Services.Abstraction
{
    public interface IUserService
    {
        Task<List<UserModel>> GetUsersAsync();
        Task<UserEntity> GetUserByIdAsync(long userId);
        Task<List<UserWithLastMessage>> GetUserswithLastMessageAsync(long userForId);
        Task<UserEntity?> GetUserByUsernameAndPasswordAsync(string userName, string password);
        Task<long> RegisterUserAsync(RegisterUserRequest user);
        Task<long> SaveUserAsync(SaveUserRequest user);
    }
}
