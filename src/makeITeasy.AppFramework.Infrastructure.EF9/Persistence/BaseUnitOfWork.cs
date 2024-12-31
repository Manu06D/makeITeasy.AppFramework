using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace makeITeasy.AppFramework.Infrastructure.EF9.Persistence
{
    public class BaseUnitOfWork<T> where T : DbContext
    {
        protected readonly IMapper _mapper;
        protected DbContext _context;
        protected Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
        protected ILogger<BaseUnitOfWork<T>> _logger;

        public BaseUnitOfWork(IDbContextFactory<T> dbFactory, IMapper mapper, ILogger<BaseUnitOfWork<T>> logger)
        {
            _context = dbFactory.CreateDbContext();
            _mapper = mapper;
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
                _logger.LogError("An error has occured while commiting unit of work", exception);
                return -1;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
