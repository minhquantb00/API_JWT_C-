namespace API_JWT_C_.Payloads.DTOs
{
    public class PostDTO
    {
        public string PostTitle { get; set; }
        public string PostDescription { get; set; }
        public string PostImage { get; set; }
        public int NumberOfLikes { get; set; }
        public int NumberOfComments { get; set; }
        public int View { get; set; }
        public bool PinedPost { get; set; }
        public bool Approved { get; set; }
        public UserDTO User { get; set; }
    }
}
