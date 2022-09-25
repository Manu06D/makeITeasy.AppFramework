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
using Microsoft.EntityFrameworkCore.Query;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace makeITeasy.AppFramework.Infrastructure.Persistence
{
    public abstract class BaseEfRepository<T, U> : IAsyncRepository<T> where T : class, IBaseEntity where U : DbContext
    {
        private readonly IDbContextFactory<U>? _dbFactory;
        private readonly IMapper _mapper;
        private U? _dbContext = null;

        public ICurrentDateProvider DateProvider { get; set; }

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

        protected U? GetDbContext()
        {
            if (_dbContext != null)
            {
                return _dbContext;
            }

            return _dbFactory?.CreateDbContext();
        }

        public virtual async Task<T> GetByIdAsync(object id)
        {
            if (id.GetType().IsArray)
            {
                //not so elegant, need to investigate on more suitable solution
                Array a = (Array)id;

                return a.Length switch
                {
                    1 => await GetDbContext().Set<T>().FindAsync(a.GetValue(0)),
                    2 => await GetDbContext().Set<T>().FindAsync(a.GetValue(0), a.GetValue(1)),
                    3 => await GetDbContext().Set<T>().FindAsync(a.GetValue(0), a.GetValue(1), a.GetValue(2)),
                    4 => await GetDbContext().Set<T>().FindAsync(a.GetValue(0), a.GetValue(1), a.GetValue(2), a.GetValue(3)),
                    5 => await GetDbContext().Set<T>().FindAsync(a.GetValue(0), a.GetValue(1), a.GetValue(2), a.GetValue(3), a.GetValue(4)),
                    _ => throw new Exception("Composite key with more than 5 columns are not supported"),
                };
            }

            return await GetDbContext().Set<T>().FindAsync(id);
        }

        public virtual async Task<T> GetByIdAsync(object id, List<Expression<Func<T, object>>> includes)
        {
            U? dbContext = GetDbContext();

            if (includes != null && dbContext != null)
            {
                var keyProperty = dbContext.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties[0];

                var dbSet = dbContext.Set<T>().AsQueryable();

                //TODO : test if it works :)
                dbSet = includes.Aggregate(dbSet, (current, include) => current.Include(include));

                return await dbSet.FirstOrDefaultAsync(e => EF.Property<object>(e, keyProperty.Name) == id);
            }

            return await GetByIdAsync(id);
        }

        public virtual async Task<IList<T>> ListAllAsync(List<Expression<Func<T, object>>>? includes = null)
        {
            var dbSet = GetDbContext().Set<T>().AsQueryable();

            if (includes != null)
            {
                dbSet = includes.Aggregate(dbSet, (current, include) => current.Include(include));
            }

            return await dbSet.ToListAsync();
        }

        public virtual async Task<QueryResult<T>> ListAsync(ISpecification<T> spec, bool includeCount = false)
        {
            QueryResult<T> result = new();

            (result.TotalItems, IQueryable<T> filteredSet) = await CreateQueryableFromSpec(spec, GetDbContext(), includeCount);

            result.Results = await filteredSet.AsNoTracking().ToListAsync();

            return result;
        }

        public virtual async Task<QueryResult<X>> ListWithProjectionAsync<X>(ISpecification<T> spec, bool includeCount = false) where X : class
        {
            QueryResult<X> result = new();

            (result.TotalItems, IQueryable<T> filteredSet) = await CreateQueryableFromSpec(spec, GetDbContext(), includeCount);

            result.Results = filteredSet.AsNoTracking().ProjectTo<X>(_mapper.ConfigurationProvider).DecompileAsync().ToList();

            return result;
        }

        private async Task<(int, IQueryable<T>)> CreateQueryableFromSpec(ISpecification<T> spec, U dbContext, bool includeCount = false)
        {
            if (typeof(IIsValidSpecification).IsAssignableFrom(spec.GetType()) && !((IIsValidSpecification)spec).IsValid())
            {
                throw new InvalidQueryException();
            }

            IQueryable<T> filteredSet = BaseEfRepository<T, U>.ApplySpecification(spec, dbContext);

            int totalItems = await ApplyCountIfNeededAsync(filteredSet, includeCount);

            if (spec.IsPagingEnabled && spec.Take.GetValueOrDefault() > 0)
            {
                filteredSet = BaseEfRepository<T, U>.ApplyPaging(filteredSet, spec);
            }

            return (totalItems, filteredSet);
        }

        public virtual async Task<int> ApplyCountIfNeededAsync(IQueryable<T> filteredSet, bool includeCount)
        {
            if (includeCount)
            {
                return await filteredSet.CountAsync().ConfigureAwait(false);
            }

            return 0;
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await BaseEfRepository<T, U>.ApplySpecification(spec, GetDbContext()).CountAsync();
        }

        public async Task<T> AddAsync(T entity, bool saveChanges = true)
        {
            U dbContext = GetDbContext();

            PrepareEntityForDbOperation(entity, EntityState.Added);

            await dbContext.Set<T>().AddAsync(entity);

            await SaveOrUpdateChanges(dbContext, saveChanges);

            return entity;
        }

        public async Task<ICollection<T>> AddRangeAsync(ICollection<T> entities, bool saveChanges = true)
        {
            U dbContext = GetDbContext();

            PrepareEntitiesForDbOperation(entities, EntityState.Added);

            await dbContext.Set<T>().AddRangeAsync(entities);

            await SaveOrUpdateChanges(dbContext, saveChanges);

            return entities;
        }

        private async Task<int> SaveOrUpdateChanges(U dbContext, bool saveChanges = true)
        {
            var entries =
                dbContext.ChangeTracker.Entries().Where(e => e.Entity is ITimeTrackingEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var hasAnyChangeOnEntry = true;

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
                return await dbContext.SaveChangesAsync();
            }

            return -1;
        }

        private bool PrepareEntityForDbOperation(T entity, EntityState state)
        {
            DateTime now = DateProvider?.Now ?? DateTime.Now;

            (var action, bool recursive) = GetITimeTrackingAction(state, now);

            bool dateTimeUpdated = BaseEfRepository<T, U>.UpdateITimeTrackingEntity(entity, action, recursive);

            return dateTimeUpdated;
        }

        private (Action<ITimeTrackingEntity> action, bool recursive) GetITimeTrackingAction(EntityState state, DateTime date)
        {
            Action<ITimeTrackingEntity> action = null;
            bool recursive = false;

            if (state == EntityState.Added)
            {
                action = (x) => { x.CreationDate = date; x.LastModificationDate = date; };
                recursive = true;
            }
            else if (state == EntityState.Modified)
            {
                action = (x) => { x.LastModificationDate = date; };
            }

            return (action, recursive);
        }

        private void PrepareEntitiesForDbOperation(ICollection<T> entities, EntityState state)
        {
            DateTime now = DateProvider?.Now ?? DateTime.Now;

            (var action, bool recursive) = GetITimeTrackingAction(state, now);

            UpdateITimeTrackingEntities(entities, action, recursive);
        }

        private void UpdateITimeTrackingEntities(ICollection<T> entities, Action<ITimeTrackingEntity> action, bool recursive = true)
        {
            foreach (var entity in entities)
            {
                BaseEfRepository<T, U>.UpdateITimeTrackingEntity(entity, action, recursive);
            }
        }

        private static bool UpdateITimeTrackingEntity(IBaseEntity entity, Action<ITimeTrackingEntity> action, bool recursive = true)
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

        private async Task<CommandResult<T>> SaveOrUpdateChangesWithResult(T entity, U dbContext, bool saveChanges = true)
        {
            var result = new CommandResult<T>();

            int dbChanges = await SaveOrUpdateChanges(dbContext, saveChanges);

            result.Entity = entity;
            result.Result = dbChanges > 0 ? CommandState.Success : CommandState.Warning;

            return result;
        }

        /// <summary>
        /// Update Entity. Only entity will be 
        /// </summary>'
        /// <param name="entity"></param>
        /// <param name="saveChanges"></param>
        /// <returns></returns>
        public async Task<CommandResult<T>> UpdateAsync(T entity, bool saveChanges = true)
        {
            var dbContext = GetDbContext();

            if (dbContext.Entry(entity).State == EntityState.Detached)
            {
                //This can be an issue if input entity is not fully filled with database value. Data can be lost !!
                T databaseEntity = await dbContext.FindAsync<T>(entity.DatabaseID);

                if (databaseEntity != null)
                {
                    PrepareEntityForDbOperation(entity, EntityState.Modified);

                    EntityEntry<T> ee = dbContext.Entry(databaseEntity);

                    ee.CurrentValues.SetValues(entity);
                }
            }

            var dbResult = await SaveOrUpdateChangesWithResult(entity, dbContext, saveChanges);

            return dbResult;
        }

        public async Task<int> UpdateRangeAsync(Expression<Func<T, bool>> entityPredicate, List<PropertyChangeCollection<T, object>> changes)
        {
            IQueryable<T> query = GetDbContext().Set<T>().Where(entityPredicate);

            Func<SetPropertyCalls<T>, SetPropertyCalls<T>> propertyCalls = x => changes.Aggregate(x, (current, change) => current.SetProperty(change.Property, change.UpdateExpression));

            //propertyCalls = changes.Aggregate(changes, (current, change) => current.SetProperty(change.Property, change.UpdateExpression));

            //foreach(var change in changes)
            //{
            //    propertyCalls = x => x.SetProperty(change.Property, change.UpdateExpression);
            //}

            //s => changes..Aggregate(s, (current, change) => current.SetProperty(change.Property, change.UpdateExpression));

            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> exp =
                  x => x.SetProperty(changes[0].Property, changes[0].UpdateExpression)
                //changes.Aggregate(changes, (current, change) => current.SetProperty(change.Property, change.UpdateExpression));
                //x => changes.AsEnumerable().Aggregate(x, (current, change) => current.SetProperty(change.Property, change.UpdateExpression))

            ;

            //exp  = Expression.Lambda<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>(exp.Body);
            //exp = Expression.Lambda<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>(Expression.Call(propertyCalls.Method));//

            return await query.ExecuteUpdateAsync(exp);
       }

        public async Task<CommandResult<T>> UpdatePropertiesAsync(T entity, string[] propertyNames, bool saveChanges = true)
        {
            var dbContext = GetDbContext();

            var result = new CommandResult<T>();
            T databaseEntity = await GetByIdAsync(entity.DatabaseID);

            if (PrepareEntityForDbOperation(entity, EntityState.Modified))
            {
                Array.Resize(ref propertyNames, propertyNames.Length + 1);
                propertyNames[propertyNames.Length - 1] = nameof(ITimeTrackingEntity.LastModificationDate);
            }

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
            CommandResult result = new();

            U dbContext = GetDbContext();

            dbContext.Set<T>().Remove(entity);

            if (saveChanges)
            {
                int dbResult = await dbContext.SaveChangesAsync();
                result.Result = dbResult > 0 ? CommandState.Success : CommandState.Error;
            }

            return result;
        }

        private static IQueryable<T> ApplySpecification(ISpecification<T> spec, U dbContext)
        {
            return SpecificationEvaluator<T>.GetQuery(dbContext.Set<T>().AsQueryable(), spec);
        }

        private static IQueryable<T> ApplyPaging(IQueryable<T> filteredSet, ISpecification<T> spec)
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
