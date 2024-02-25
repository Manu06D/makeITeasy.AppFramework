﻿using AutoMapper;
using AutoMapper.QueryableExtensions;

using DelegateDecompiler.EntityFrameworkCore;

using EFCore.BulkExtensions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.AppFramework.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace makeITeasy.AppFramework.Infrastructure.EF7.Persistence
{
    public abstract class BaseEfRepository<T, U> : IAsyncRepository<T> where T : class, IBaseEntity where U : DbContext
    {
        private readonly IDbContextFactory<U>? _dbFactory;
        private readonly IMapper _mapper;
        private readonly U? _dbContext = null;

        public ICurrentDateProvider? DateProvider { get; set; }

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

            if(_dbFactory == null)
            {
                throw new Exception("DBFactory has not been registered.");
            }

            return _dbFactory.CreateDbContext();
        }

        public virtual async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            if (id.GetType().IsArray)
            {
                //not so elegant, need to investigate on more suitable solution
                Array a = (Array)id;

                if(a == null)
                {
                    throw new Exception("An error has occured while casting the primary key");
                }

                return a.Length switch
                {
                    1 => await GetDbContext().Set<T>().FindAsync(a.GetValue(0), cancellationToken),
                    2 => await GetDbContext().Set<T>().FindAsync(a.GetValue(0), a.GetValue(1), cancellationToken),
                    3 => await GetDbContext().Set<T>().FindAsync(a.GetValue(0), a.GetValue(1), a.GetValue(2), cancellationToken),
                    4 => await GetDbContext().Set<T>().FindAsync(a.GetValue(0), a.GetValue(1), a.GetValue(2), a.GetValue(3), cancellationToken),
                    5 => await GetDbContext().Set<T>().FindAsync(a.GetValue(0), a.GetValue(1), a.GetValue(2), a.GetValue(3), a.GetValue(4), cancellationToken),
                    _ => throw new Exception("Composite key with more than 5 columns are not supported"),
                };
            }

            return await GetDbContext().Set<T>().FindAsync(id);
        }

        public virtual async Task<T?> GetByIdAsync(object id, List<Expression<Func<T, object>>>? includes, CancellationToken cancellationToken = default)
        {
            U? dbContext = GetDbContext();

            if (includes != null && dbContext != null)
            {
                IProperty? keyProperty = dbContext.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties[0];
                if (keyProperty == null)
                {
                    throw new Exception($"An error has occred while guessing the primary key of object {typeof(T).FullName}"); ;
                }

                IQueryable<T> dbSet = dbContext.Set<T>().AsQueryable();

                //TODO : test if it works :)
                dbSet = includes.Aggregate(dbSet, (current, include) => current.Include(include));

                return await dbSet.FirstOrDefaultAsync(e => EF.Property<object>(e, keyProperty.Name) == id, cancellationToken);
            }

            return await GetByIdAsync(id, cancellationToken);
        }

        public virtual async Task<IList<T>> ListAllAsync(List<Expression<Func<T, object>>>? includes = null, CancellationToken cancellationToken = default)
        {
            var dbSet = GetDbContext().Set<T>().AsQueryable();

            if (includes != null)
            {
                dbSet = includes.Aggregate(dbSet, (current, include) => current.Include(include));
            }

            return await dbSet.ToListAsync(cancellationToken);
        }

        public virtual async Task<QueryResult<T>> ListAsync(ISpecification<T> spec, bool includeCount = false, CancellationToken cancellationToken = default)
        {
            QueryResult<T> result = new();

            (result.TotalItems, IQueryable<T>? filteredSet) = await CreateQueryableFromSpec(spec, GetDbContext(), includeCount, cancellationToken);

            if(filteredSet != null)
            {
                result.Results = await filteredSet.AsNoTracking().ToListAsync(cancellationToken);
            }

            return result;
        }

        public virtual async Task<QueryResult<X>> ListWithProjectionAsync<X>(ISpecification<T> spec, bool includeCount = false, CancellationToken cancellationToken = default) where X : class
        {
            QueryResult<X> result = new();

            (result.TotalItems, IQueryable<T>? filteredSet) = await CreateQueryableFromSpec(spec, GetDbContext(), includeCount, cancellationToken);

            if (filteredSet != null && _mapper?.ConfigurationProvider != null)
            {
                result.Results = filteredSet.AsNoTracking().ProjectTo<X>(_mapper.ConfigurationProvider).DecompileAsync().ToList();
            }

            return result;
        }

        private async Task<(int, IQueryable<T>?)> CreateQueryableFromSpec(ISpecification<T> spec, U dbContext, bool includeCount = false, CancellationToken cancellationToken = default)
        {
            if (typeof(IIsValidSpecification).IsAssignableFrom(spec.GetType()) && !((IIsValidSpecification)spec).IsValid())
            {
                throw new InvalidQueryException();
            }

            IQueryable<T>? filteredSet = BaseEfRepository<T, U>.ApplySpecification(spec, dbContext);

            int totalItems = await ApplyCountIfNeededAsync(filteredSet, includeCount, cancellationToken);

            if (spec.IsPagingEnabled && spec.Take.GetValueOrDefault() > 0)
            {
                filteredSet = BaseEfRepository<T, U>.ApplyPaging(filteredSet, spec);
            }

            return (totalItems, filteredSet);
        }

        public virtual async Task<int> ApplyCountIfNeededAsync(IQueryable<T>? filteredSet, bool includeCount, CancellationToken cancellationToken = default)
        {
            if (includeCount && filteredSet != null)
            {
                return await filteredSet.CountAsync(cancellationToken).ConfigureAwait(false);
            }

            return 0;
        }

        public async Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            IQueryable<T>? query = BaseEfRepository<T, U>.ApplySpecification(spec, GetDbContext());

            if (query != null)
            {
                return await query.CountAsync(cancellationToken);
            }
            else
            {
                return -1;
            }
        }

        public async Task<T> AddAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            U dbContext = GetDbContext();

            PrepareEntityForDbOperation(entity, EntityState.Added);

            await dbContext.Set<T>().AddAsync(entity, cancellationToken);

            await SaveOrUpdateChanges(dbContext, saveChanges);

            return entity;
        }

        public async Task<ICollection<T>> AddRangeAsync(ICollection<T> entities, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            U dbContext = GetDbContext();

            PrepareEntitiesForDbOperation(entities, EntityState.Added);

            await dbContext.Set<T>().AddRangeAsync(entities, cancellationToken);

            await SaveOrUpdateChanges(dbContext, saveChanges, cancellationToken);

            return entities;
        }

        private async Task<int> SaveOrUpdateChanges(U dbContext, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            var entries =
                dbContext.ChangeTracker.Entries().Where(e => e.Entity is ITimeTrackingEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                bool hasAnyChangeOnEntry = true;

                if (entry.State == EntityState.Modified)
                {
                    List<string> propertiesToExclude = typeof(ITimeTrackingEntity).GetProperties().Select(x => x.Name).ToList();

                    foreach (var property in entry.Properties.Where(x => !propertiesToExclude.Contains(x.Metadata.Name)))
                    {
                        bool hasLocalChange = false;
                        if (property.OriginalValue == null)
                        {
                            hasLocalChange = property.CurrentValue != null;
                        }
                        else
                        {
                            hasLocalChange = !property.OriginalValue.Equals(property.CurrentValue);
                        }

                        if (hasLocalChange)
                        {
                            hasAnyChangeOnEntry = true;
                            break;
                        }

                        hasAnyChangeOnEntry = false;
                    }

                    if (!hasAnyChangeOnEntry)
                    {
                        //revert LastModificationDateChange
                        entry.Property(nameof(ITimeTrackingEntity.LastModificationDate)).IsModified = false;
                    }
                }

                if (hasAnyChangeOnEntry)
                {
                    DateTime now = DateProvider?.Now ?? DateTime.Now;

                    ((ITimeTrackingEntity)entry.Entity).LastModificationDate = now;

                    if (entry.State == EntityState.Added)
                    {
                        ((ITimeTrackingEntity)entry.Entity).CreationDate = now;
                    }
                }
            }

            if (saveChanges)
            {
                return await dbContext.SaveChangesAsync(cancellationToken);
            }

            return -1;
        }

        private bool PrepareEntityForDbOperation(T entity, EntityState state)
        {
            DateTime now = DateProvider?.Now ?? DateTime.Now;

            (var action, bool recursive) = BaseEfRepository<T, U>.GetITimeTrackingAction(state, now);

            bool dateTimeUpdated = BaseEfRepository<T, U>.UpdateITimeTrackingEntity(entity, action);

            return dateTimeUpdated;
        }

        private static (Action<ITimeTrackingEntity> action, bool recursive) GetITimeTrackingAction(EntityState state, DateTime date)
        {
            Action<ITimeTrackingEntity>? action = null;
            bool recursive = false;

            switch (state)
            {
                case EntityState.Added:
                    {
                        action = (x) => { x.CreationDate = date; x.LastModificationDate = date; };
                        recursive = true;
                        break;
                    }

                case EntityState.Modified:
                    {
                        action = (x) => { x.LastModificationDate = date; };
                        break;
                    }

                default:
                    {
                        action = (x) => { };
                        break;
                    }
            }

            return (action, recursive);
        }

        private void PrepareEntitiesForDbOperation(ICollection<T> entities, EntityState state)
        {
            DateTime now = DateProvider?.Now ?? DateTime.Now;

            (var action, bool recursive) = BaseEfRepository<T, U>.GetITimeTrackingAction(state, now);

            BaseEfRepository<T, U>.UpdateITimeTrackingEntities(entities, action);
        }

        private static void UpdateITimeTrackingEntities(ICollection<T> entities, Action<ITimeTrackingEntity> action)
        {
            foreach (var entity in entities)
            {
                BaseEfRepository<T, U>.UpdateITimeTrackingEntity(entity, action);
            }
        }

        private static bool UpdateITimeTrackingEntity(IBaseEntity entity, Action<ITimeTrackingEntity> action)
        {
            bool result = false;

            if (entity == null || action == null)
            {
                return result;
            }

            Type iTimeTrackingType = typeof(ITimeTrackingEntity);

            if (iTimeTrackingType.IsAssignableFrom(entity.GetType()))
            {
                action((ITimeTrackingEntity)entity);

                result = true;
            }

            ////deep dive in child to set date
            //foreach (var property in entity.GetType().GetProperties())
            //{
            //    if (recursive && property.PropertyType.IsClass && typeof(IBaseEntity).IsAssignableFrom(property.PropertyType))
            //    {
            //        UpdateITimeTrackingEntity((IBaseEntity)property.GetValue(entity), action);
            //    }
            //    //else if (recursive && property.PropertyType.IsGenericType && property.PropertyType.GetInterface(nameof(System.Collections.IEnumerable)) != null)
            //    //{
            //    //    if (property.PropertyType.GetGenericArguments().Any(x => iTimeTrackingType.IsAssignableFrom(x)))
            //    //    {
            //    //        foreach (var x in (System.Collections.IEnumerable)property.GetValue(entity))
            //    //        {
            //    //            //set to false otherwise we can go into an infinite loop
            //    //            UpdateITimeTrackingEntity(x as IBaseEntity, action, false);
            //    //        }
            //    //    }
            //    //}
            //}

            return result;
        }

        private async Task<CommandResult<T>> SaveOrUpdateChangesWithResult(T entity, U dbContext, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            var result = new CommandResult<T>();

            int dbChanges = await SaveOrUpdateChanges(dbContext, saveChanges, cancellationToken);

            result.Entity = entity;
            result.Result = dbChanges >= 0 ? CommandState.Success : CommandState.Error;

            return result;
        }

        /// <summary>
        /// Update Entity. Only entity will be 
        /// </summary>'
        /// <param name="entity"></param>
        /// <param name="saveChanges"></param>
        /// <returns></returns>
        public async Task<CommandResult<T>> UpdateAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            var dbContext = GetDbContext();

            if (dbContext.Entry(entity).State == EntityState.Detached)
            {
                //This can be an issue if input entity is not fully filled with database value. Data can be lost !!
                T? databaseEntity = await dbContext.FindAsync<T>(entity.DatabaseID, cancellationToken);

                if (databaseEntity != null)
                {
                    PrepareEntityForDbOperation(entity, EntityState.Modified);

                    EntityEntry<T> ee = dbContext.Entry(databaseEntity);

                    ee.CurrentValues.SetValues(entity);
                }
            }

            var dbResult = await SaveOrUpdateChangesWithResult(entity, dbContext, saveChanges, cancellationToken);

            return dbResult;
        }

        public async Task<int> UpdateRangeAsync(Expression<Func<T, bool>> entityPredicate, Expression<Func<T, T>> updateExpression, CancellationToken cancellationToken = default)
        {

            return await GetDbContext().Set<T>().Where(entityPredicate).BatchUpdateAsync(updateExpression, cancellationToken: cancellationToken);
        }

        public async Task<CommandResult<T>> UpdatePropertiesAsync(T entity, string[] propertyNames, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            if (propertyNames == null || propertyNames.Length  == 0)
            {
                throw new ArgumentNullException(nameof(propertyNames));
            }

            var dbContext = GetDbContext();

            var result = new CommandResult<T>();
            T? databaseEntity = await GetByIdAsync(entity.DatabaseID, cancellationToken);

            if (PrepareEntityForDbOperation(entity, EntityState.Modified))
            {
                Array.Resize(ref propertyNames, propertyNames.Length + 1);
                propertyNames[^1] = nameof(ITimeTrackingEntity.LastModificationDate);
            }

            if (databaseEntity != null)
            {
                bool shouldSave = false;

                foreach (PropertyInfo property in GetPropertiesToUpdate(propertyNames))
                {
                    if (property != null)
                    {
                        object? oldValue = property.GetValue(databaseEntity);
                        object? newValue = property.GetValue(entity);

                        if (
                            oldValue?.Equals(newValue) == false
                            ||
                            oldValue == null && newValue != null)
                        {
                            property.SetValue(databaseEntity, newValue);
                            dbContext.Entry(databaseEntity).Property(property.Name).IsModified = true;
                            shouldSave = true;
                        }
                    }
                }

                if (shouldSave)
                {
                    result = await SaveOrUpdateChangesWithResult(entity, dbContext, shouldSave, cancellationToken);
                }
                else
                {
                    result.Result = CommandState.Success;
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

        public async Task<CommandResult> DeleteAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            CommandResult result = new();

            U dbContext = GetDbContext();

            dbContext.Set<T>().Remove(entity);

            if (saveChanges)
            {
                int dbResult = await dbContext.SaveChangesAsync(cancellationToken);
                result.Result = dbResult > 0 ? CommandState.Success : CommandState.Error;
            }

            return result;
        }

        private static IQueryable<T>? ApplySpecification(ISpecification<T> spec, U dbContext)
        {
            return SpecificationEvaluator<T>.GetQuery(dbContext.Set<T>().AsQueryable(), spec);
        }

        private static IQueryable<T>? ApplyPaging(IQueryable<T>? filteredSet, ISpecification<T> spec)
        {
            if (spec is null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            return filteredSet?.Skip(spec.Skip.GetValueOrDefault()).Take(spec.Take.GetValueOrDefault(10));
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
