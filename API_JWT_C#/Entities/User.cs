using API_JWT_C_.Enumerates;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_JWT_C_.Entities
{
    [Table("Users_tbl")]
    [Index("UserName", IsUnique = true)]
    public class User : BaseEntity
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; } = nameof(Enumerates.Gender.UNDEFINE);
        public string AvatarUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
    }
}
