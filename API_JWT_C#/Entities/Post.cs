using System.ComponentModel.DataAnnotations.Schema;

namespace API_JWT_C_.Entities
{
    [Table("Post_tbl")]
    public class Post : BaseEntity
    {
        public string PostTitle { get; set; }
        public string PostDescription { get; set; }
        public string PostImage { get; set; }
        public int NumberOfLikes { get; set; } = 0;
        public int NumberOfComments { get; set; } = 0;
        public int View { get; set; } = 0;
        public bool PinedPost { get; set; } = false;
        public bool Approved { get; set; } = false;
        public int UserId { get; set; }
        public User User { get; set; }
        public int PostTypeId { get; set; }
        public PostType PostType { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
    }
}
