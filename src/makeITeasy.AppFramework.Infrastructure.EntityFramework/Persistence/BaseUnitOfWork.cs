using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace makeITeasy.AppFramework.Infrastructure.EntityFramework.Persistence
{
    public class BaseUnitOfWork<T> where T : DbContext
    {
        protected DbContext _context;
        protected Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
        protected ILogger<BaseUnitOfWork<T>> _logger;

        public BaseUnitOfWork(IDbContextFactory<T> dbFactory, ILogger<BaseUnitOfWork<T>> logger)
        {
            _context = dbFactory.CreateDbContext();
            _logger = logger;
        }

        public async Task<int> CommitAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error has occured while commiting unit of work");
                return -1;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
