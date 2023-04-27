namespace Contracts.Requests
{
    public class SaveUserRequest
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
    }
}
