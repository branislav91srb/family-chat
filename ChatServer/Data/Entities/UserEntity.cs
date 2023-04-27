using System.ComponentModel.DataAnnotations;

namespace ChatServer.Data.Entities
{
    public class UserEntity
    {
        [Key]
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
    }
}
