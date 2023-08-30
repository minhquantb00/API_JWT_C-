using API_JWT_C_.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_JWT_C_.DataContext
{
    public class AppDbContext : DbContext
    {
        //public AppDbContext(DbContextOptions<AppDbContext> options)
        //: base(options)
        //{
        //}
        //public AppDbContext() { }
        public DbSet<User> users {  get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<RefreshToken> refreshTokens { get; set; }
        public DbSet<PostType> postTypes { get; set; }
        public DbSet<Post> posts { get; set; }
        public DbSet<Comment> comments { get; set; }
        public DbSet<ConfirmEmail> confirmEmails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(SourseData.MyConnect());
        }
    }
}
