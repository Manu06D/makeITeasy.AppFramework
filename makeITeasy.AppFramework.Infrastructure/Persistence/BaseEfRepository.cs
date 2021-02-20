using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DelegateDecompiler.EntityFrameworkCore;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using makeITeasy.AppFramework.Core.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using makeITeasy.AppFramework.Core.Commands;

namespace makeITeasy.AppFramework.Infrastructure.Persistence
{
    public abstract class BaseEfRepository<T, U> : IAsyncRepository<T> where T : class, IBaseEntity where U : DbContext
    {
        private readonly IDbContextFactory<U> _dbFactory;
        private readonly IMapper _mapper;
        private U _dbContext = null;

        protected BaseEfRepository(IDbContextFactory<U> dbFactory, IMapper mapper)
        {
            _dbFactory = dbFactory;
            _mapper = mapper;
        }

        protected BaseEfRepository(U dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        protected U GetDbContext()
        {
            if (_dbContext != null)
            {
                return _dbContext;
            }

            return _dbFactory.CreateDbContext();
        }

        public virtual async Task<T> GetByIdAsync(object id)
        {
            return await GetDbContext().Set<T>().FindAsync(id);
        }

        public virtual async Task<T> GetByIdAsync(object id, List<Expression<Func<T, object>>> includes)
        {
            U dbContext = GetDbContext();
            if (includes != null)
            {
                var keyProperty = dbContext.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties[0];

                var dbSet = dbContext.Set<T>().AsQueryable();

                //TODO : test if it works :)
                dbSet = includes.Aggregate(dbSet, (current, include) => current.Include(include));

                return await dbSet.FirstOrDefaultAsync(e => EF.Property<object>(e, keyProperty.Name) == id);
            }

            return await GetByIdAsync(id);
        }

        public async Task<IList<T>> ListAllAsync()
        {
            U dbContext = GetDbContext();
            return await dbContext.Set<T>().ToListAsync();
        }

        public async Task<QueryResult<T>> ListAsync(ISpecification<T> spec, bool includeCount = false)
        {
            QueryResult<T> result = new QueryResult<T>();

            (int nbResult, IQueryable<T> filteredSet) = await CreateQueryableFromSpec(spec, GetDbContext(), includeCount);

            result.TotalItems = nbResult;

            result.Results = await filteredSet.AsNoTracking().ToListAsync();

            return result;
        }

        public async Task<QueryResult<X>> ListWithProjectionAsync<X>(ISpecification<T> spec, bool includeCount = false) where X : class
        {
            QueryResult<X> result = new QueryResult<X>();

            (int nbResult, IQueryable<T> filteredSet) = await CreateQueryableFromSpec(spec, GetDbContext(), includeCount);

            result.TotalItems = nbResult;
            result.Results = filteredSet.AsNoTracking().ProjectTo<X>(_mapper.ConfigurationProvider).DecompileAsync().ToList();

            return result;
        }

        private async Task<(int, IQueryable<T>)> CreateQueryableFromSpec(ISpecification<T> spec, U dbContext, bool includeCount = false)
        {
            IQueryable<T> filteredSet = ApplySpecification(spec, dbContext);

            int totalItems = await ApplyCountIfNeededAsync(filteredSet, includeCount);

            if (spec.IsPagingEnabled && spec.Take.GetValueOrDefault() > 0)
            {
                filteredSet = ApplyPaging(filteredSet, spec);
            }

            return (totalItems, filteredSet);
        }

        private async Task<int> ApplyCountIfNeededAsync(IQueryable<T> filteredSet, bool includeCount)
        {
            if (includeCount)
            {
                return await filteredSet.CountAsync();
            }

            return 0;
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec, GetDbContext()).CountAsync();
        }

        public async Task<T> AddAsync(T entity, bool saveChanges = true)
        {
            U dbContext = GetDbContext();
            await dbContext.Set<T>().AddAsync(entity);

            await SaveOrUpdateChanges(entity, dbContext, saveChanges);

            return entity;
        }

        private async Task<int> SaveOrUpdateChanges(T entity, U dbContext, bool saveChanges = true)
        {
            var entries =
                dbContext.ChangeTracker.Entries().Where(e => e.Entity is ITimeTrackingEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                DateTime now = DateTime.Now;

                ((ITimeTrackingEntity)entry.Entity).LastModificationDate = now;
                ((ITimeTrackingEntity)entity).LastModificationDate = now;

                if (entry.State == EntityState.Added)
                {
                    ((ITimeTrackingEntity)entry.Entity).CreationDate = now;
                    ((ITimeTrackingEntity)entity).CreationDate = now;
                }
            }

            if (saveChanges)
            {
                return await dbContext.SaveChangesAsync();
            }

            return -1;
        }

        private async Task<CommandResult<T>> SaveOrUpdateChangesWithResult(T entity, U dbContext, bool saveChanges = true)
        {
            var result = new CommandResult<T>();

            int dbChanges = await SaveOrUpdateChanges(entity, dbContext, saveChanges);

            result.Entity = entity;
            result.Result = dbChanges > 0 ? CommandState.Success : CommandState.Warning;

            return result;
        }

        public async Task<CommandResult<T>> UpdateAsync(T entity, bool saveChanges = true)
        {
            var dbContext = GetDbContext();

            if (dbContext.Entry(entity).State == EntityState.Detached)
            {
                //This can be an issue if input entity is not fully filled with database value. Data can be lost !!
                T databaseEntity = await GetByIdAsync(entity.DatabaseID);
                if (databaseEntity != null)
                {
                    EntityEntry<T> ee = dbContext.Entry(databaseEntity);

                    ee.CurrentValues.SetValues(entity);

                    ee.State = EntityState.Modified;
                }
            }

            var dbResult = await SaveOrUpdateChangesWithResult(entity, dbContext, saveChanges);

            return dbResult;
        }

        public async Task<CommandResult<T>> UpdatePropertiesAsync(T entity, string[] propertyNames, bool saveChanges = true)
        {
            var dbContext = GetDbContext();

            var result = new CommandResult<T>();
            T databaseEntity = await GetByIdAsync(entity.DatabaseID);

            if (databaseEntity != null)
            {
                bool shouldSave = false;

                foreach (PropertyInfo property in GetPropertiesToUpdate(propertyNames))
                {
                    if (property != null)
                    {
                        object oldValue = property.GetValue(databaseEntity);
                        object newValue = property.GetValue(entity);

                        if (
                            (oldValue?.Equals(newValue) == false)
                            ||
                            (oldValue == null && newValue != null))
                        {
                            property.SetValue(databaseEntity, newValue);
                            dbContext.Entry(databaseEntity).Property(property.Name).IsModified = true;
                            shouldSave = true;
                        }
                    }
                }

                if (shouldSave)
                {
                    result = await SaveOrUpdateChangesWithResult(entity, dbContext, shouldSave);
                }
                else
                {
                    result.Result = CommandState.Warning;
                    result.Message = "No properties have been changed";
                }
            }

            return result;
        }

        private static PropertyInfo[] GetPropertiesToUpdate(string[] propertyNames)
        {
            PropertyInfo[] objectProperties = typeof(T).GetProperties().Where(x => x.CanRead && x.CanWrite).ToArray();

            string[] propertiesToUpdate = objectProperties.Select(x => x.Name).Intersect(propertyNames).ToArray();

            return objectProperties.Where(x => propertiesToUpdate.Contains(x.Name)).ToArray();
        }

        public async Task<CommandResult> DeleteAsync(T entity, bool saveChanges = true)
        {
            CommandResult result = new CommandResult();

            U dbContext = GetDbContext();

            dbContext.Set<T>().Remove(entity);

            if (saveChanges)
            {
                int dbResult = await dbContext.SaveChangesAsync();
                result.Result = dbResult > 0 ? CommandState.Success : CommandState.Error;
            }

            return result;
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec, U dbContext)
        {
            return SpecificationEvaluator<T>.GetQuery(dbContext.Set<T>().AsQueryable(), spec);
        }

        private IQueryable<T> ApplyPaging(IQueryable<T> filteredSet, ISpecification<T> spec)
        {
            return filteredSet.Skip(spec.Skip.Value).Take(spec.Take.Value);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}
