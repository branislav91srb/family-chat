using ChatServer.Data.Entities;
using System.Linq.Expressions;

namespace ChatServer.Services.Abstraction
{
    public interface IUserRepository
    {
        Task<List<UserEntity>> GetUsersAsync();

        Task<UserEntity?> GetUserAsync(Expression<Func<UserEntity, bool>> predicate);

        Task<UserEntity> GetUserByIdAsync(long id);

        Task<long> SaveUserAsync(UserEntity user);
    }
}
