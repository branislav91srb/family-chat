using ChatServer.Data;
using ChatServer.Data.Entities;
using ChatServer.Services.Abstraction;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace ChatServer.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly ServerDbContext _db;

        public UserRepository(ServerDbContext db)
        {
            _db = db;
        }

        public async Task<List<UserEntity>> GetUsersAsync()
        {
            return await _db.Users.ToListAsync();
        }

        public async Task<UserEntity> GetUserByIdAsync(long id)
        {
            return await _db.Users.SingleAsync(x => x.Id == id);
        }

        public async Task<UserEntity?> GetUserAsync(Expression<Func<UserEntity, bool>> predicate)
        {
            return await _db.Users.SingleOrDefaultAsync(predicate);
        }

        public async Task<long> SaveUserAsync(UserEntity user)
        {
            var existingUser = await _db.Users.SingleOrDefaultAsync(x => user.Id > 0 && x.Id == user.Id);

            if (existingUser == null)
            {
                _db.Users.Add(user);
            }
            else
            {
                existingUser.UserName = user.UserName;
                existingUser.Password = user.Password;
                existingUser.Avatar = user.Avatar;
            }

            await _db.SaveChangesAsync();

            return user.Id;
        }
    }
}