namespace API_JWT_C_.Payloads.DTOs
{
    public class UserDTO
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
