namespace API_JWT_C_.Payloads.Requests
{
    public class RegisterRequest
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IFormFile AvatarUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
