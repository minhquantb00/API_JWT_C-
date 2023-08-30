using System.ComponentModel.DataAnnotations.Schema;

namespace API_JWT_C_.Entities
{
    [Table("Role_tbl")]
    public class Role : BaseEntity
    {
        public string RoleName { get; set; }
        public IEnumerable<User> Users { get; set; }
    }
}
