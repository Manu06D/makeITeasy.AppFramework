﻿using AutoMapper;

using makeITeasy.AppFramework.Infrastructure.EF6.Persistence;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet6.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace makeITeasy.CarCatalog.dotnet6.Infrastructure.Persistence
{
    public class TransactionCarCatalogRepository<T> : TransactionEfRepository<T, CarCatalogContext> where T : class, IBaseEntity
    {
        public TransactionCarCatalogRepository(IDbContextFactory<CarCatalogContext> dbFactory, IMapper mapper) : base(dbFactory, mapper)
        {
        }
    }
}
