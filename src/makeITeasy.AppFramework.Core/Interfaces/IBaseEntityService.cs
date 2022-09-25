﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using makeITeasy.AppFramework.Models;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.AppFramework.Core.Commands;

namespace makeITeasy.AppFramework.Core.Interfaces
{
    public interface IBaseEntityService<TEntity> where TEntity : class, IBaseEntity
    {
        Task<TEntity> GetByIdAsync(object id, List<Expression<Func<TEntity, object>>> includes = null);
        Task<IList<TEntity>> ListAllAsync(List<Expression<Func<TEntity, object>>>? includes = null);
        Task<QueryResult<TEntity>> QueryAsync(ISpecification<TEntity> specification, bool includeCount = false);
        Task<TEntity> GetFirstByQueryAsync(ISpecification<TEntity> specification);
        Task<QueryResult<TTarget>> QueryWithProjectionAsync<TTarget>(ISpecification<TEntity> specification, bool includeCount = false) where TTarget : class;
        Task<TTarget> GetFirstByQueryWithProjectionAsync<TTarget>(ISpecification<TEntity> specification) where TTarget : class;
        Task<CommandResult<TEntity>> CreateAsync(TEntity entity, bool saveChanges = true);
        Task<CommandResult<TEntity>> UpdateAsync(TEntity entity);
        Task<CommandResult<TEntity>> UpdatePropertiesAsync(TEntity entity, string[] properties);
        Task<CommandResult> DeleteAsync(TEntity entity, bool saveChanges = true);
        Task<ICollection<CommandResult<TEntity>>> CreateRangeAsync(ICollection<TEntity> entities, bool saveChanges = true);
        //Task<int> UpdateRangeAsync(Expression<Func<TEntity, bool>> entityPredicate, Expression<Func<TEntity, TEntity>> updateExpression);
        Task<int> UpdateRangeAsync(Expression<Func<TEntity, bool>> entityPredicate, List<PropertyChangeCollection<TEntity, object>> changes);
        bool Validate(TEntity entity);
    }
}
