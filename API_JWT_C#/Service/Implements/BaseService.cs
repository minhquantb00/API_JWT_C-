using API_JWT_C_.DataContext;

namespace API_JWT_C_.Service.Implements
{
    public class BaseService
    {
        public readonly AppDbContext _context;
        public BaseService()
        {
            _context = new AppDbContext();
        }
    }
}
