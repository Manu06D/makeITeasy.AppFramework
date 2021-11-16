using System;
using System.Collections.Generic;

using AutoMapper;

using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Infrastructure.Persistence;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace makeITeasy.CarCatalog.Infrastructure.Persistence
{
    public class UnitOfWork : BaseUnitOfWork<CarCatalogContext>, IUnitOfWork
    {
        public UnitOfWork(IDbContextFactory<CarCatalogContext> dbFactory, IMapper mapper, ILogger<UnitOfWork> logger)
            : base(dbFactory, mapper, logger)
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
                _repositories[type] = new EfRepository<TEntity>(_context, _mapper);
            }

            return (IAsyncRepository<TEntity>)_repositories[type];
        }
    }
}
