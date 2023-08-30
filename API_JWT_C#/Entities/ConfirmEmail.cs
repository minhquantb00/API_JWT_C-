namespace API_JWT_C_.Entities
{
    public class ConfirmEmail : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime ExpiredTime { get; set; }
        public string CodeActive { get; set; }
        public bool IsConfirm { get; set; } = false;
    }
}
