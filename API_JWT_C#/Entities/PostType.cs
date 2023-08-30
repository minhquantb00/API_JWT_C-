using System.ComponentModel.DataAnnotations.Schema;

namespace API_JWT_C_.Entities
{
    [Table("Post_Type_tbl")]
    public class PostType : BaseEntity
    {
        public string Name { get; set; }
        public IEnumerable<Post> Posts { get; set;}
    }
}
