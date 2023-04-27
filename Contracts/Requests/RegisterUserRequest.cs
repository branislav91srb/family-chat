namespace Contracts.Requests
{
    public class RegisterUserRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
    }
}
