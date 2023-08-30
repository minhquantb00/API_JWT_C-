using System.ComponentModel.DataAnnotations.Schema;

namespace API_JWT_C_.Entities
{
    [Table("RefreshToken_tbl")]
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; }
        public DateTime ExpiredTime { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
