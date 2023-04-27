using ChatServer.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatServer.Data
{
    public class ServerDbContext : DbContext
    {
        public DbSet<MessageEntity> Messages { get; set; }

        public DbSet<UserEntity> Users { get; set; }

        public ServerDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageEntity>().ToTable("Messages");
            modelBuilder.Entity<UserEntity>().ToTable("Users");

            base.OnModelCreating(modelBuilder);
        }
    }
}
