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

namespace makeITeasy.AppFramework.Core.Infrastructure.Persistence
{
    public abstract class BaseEfRepository<T, U> : IAsyncRepository<T> where T : BaseEntity where U : DbContext
    {
        protected readonly U _dbContext;
        private readonly IMapper _mapper;

        protected BaseEfRepository(U dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public virtual async Task<T> GetByIdAsync(object id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public virtual async Task<T> GetByIdAsync(object id, List<Expression<Func<T, object>>> includes)
        {
            if (includes != null)
            {
                var keyProperty = _dbContext.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties[0];

                var dbSet = _dbContext.Set<T>().AsQueryable();

                //TODO : test if it works :)
                dbSet = includes.Aggregate(dbSet, (current, include) => current.Include(include));

                return await dbSet.FirstOrDefaultAsync(e => EF.Property<object>(e, keyProperty.Name) == id);
            }

            return await GetByIdAsync(id);
        }

        public async Task<IList<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<QueryResult<T>> ListAsync(ISpecification<T> spec, bool includeCount = false)
        {
            QueryResult<T> result = new QueryResult<T>();

            (int nbResult, IQueryable<T> filteredSet) = await CreateQueryableFromSpec(spec, includeCount);

            result.TotalItems = nbResult;

            result.Results = await filteredSet.AsNoTracking().ToListAsync();

            return result;
        }

        public async Task<QueryResult<X>> ListWithProjectionAsync<X>(ISpecification<T> spec, bool includeCount = false) where X : class
        {
            QueryResult<X> result = new QueryResult<X>();

            (int nbResult, IQueryable<T> filteredSet) = await CreateQueryableFromSpec(spec, includeCount);

            result.TotalItems = nbResult;
            result.Results = filteredSet.AsNoTracking().ProjectTo<X>(_mapper.ConfigurationProvider).DecompileAsync().ToList();

            return result;
        }

        private async Task<(int, IQueryable<T>)> CreateQueryableFromSpec(ISpecification<T> spec, bool includeCount = false)
        {
            IQueryable<T> filteredSet = ApplySpecification(spec);

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
            return await ApplySpecification(spec).CountAsync();
        }

        public async Task<T> AddAsync(T entity, bool saveChanges = true)
        {
            await _dbContext.Set<T>().AddAsync(entity);

            if (saveChanges)
            {
                await _dbContext.SaveChangesAsync();
            }

            return entity;
        }

        public async Task<CommandResult<T>> UpdateAsync(T entity)
        {
            var result = new CommandResult<T>();

            if (_dbContext.Entry(entity).State == EntityState.Detached)
            {
                //This can be an issue if input entity is not fully filled with database value. Data can be lost !!
                T databaseEntity = await GetByIdAsync(entity.DatabaseID);
                if (databaseEntity != null)
                {
                    EntityEntry<T> ee = _dbContext.Entry(databaseEntity);

                    ee.CurrentValues.SetValues(entity);
                }
            }

            int dbChanges = await _dbContext.SaveChangesAsync();

            result.Entity = entity;
            result.Result = dbChanges > 0 ? CommandState.Success : CommandState.Warning;

            return result;
        }

        public async Task<CommandResult<T>> UpdatePropertiesAsync(T entity, string[] propertyNames)
        {
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
                            _dbContext.Entry(databaseEntity).Property(property.Name).IsModified = true;
                            shouldSave = true;
                        }
                    }
                }

                if (shouldSave)
                {
                    result = await UpdateAsync(databaseEntity);
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

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
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
                _dbContext.Dispose();
            }
        }
    }
}
