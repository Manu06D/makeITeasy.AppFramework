using System;
using System.Collections.Generic;

using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Infrastructure.EntityFramework.Persistence;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet9.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace makeITeasy.CarCatalog.dotnet9.Infrastructure.Persistence
{
    public class UnitOfWork : BaseUnitOfWork<CarCatalogContext>, IUnitOfWork
    {
        public UnitOfWork(IDbContextFactory<CarCatalogContext> dbFactory, ILogger<UnitOfWork> logger)
            : base(dbFactory, logger)
        {
        }

        public IAsyncRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IBaseEntity
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<Type, object>();
            }

            var type = typeof(TEntity);

            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new EfRepository<TEntity>(_context);
            }

            return (IAsyncRepository<TEntity>)_repositories[type];
        }
    }
}
