using System.ComponentModel.DataAnnotations.Schema;

namespace API_JWT_C_.Entities
{
    [Table("Comment_tbl")]
    public class Comment : BaseEntity
    {
        public string Content { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime UpdateTime { get; set; }
        public DateTime RemoveTime { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public User User { get; set; }
        public Post Post { get; set; }
    }
}
