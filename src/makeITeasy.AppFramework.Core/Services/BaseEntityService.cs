using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Models.Exceptions;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace makeITeasy.AppFramework.Core.Services
{
    public class BaseEntityService<TEntity> : IBaseEntityService<TEntity>, IDisposable where TEntity : class, IBaseEntity
    {
        //injected by autofac by propertieswired
        protected IValidator<TEntity>? _validator { get; set; }
        //injected by autofac by propertieswired
        public IAsyncRepository<TEntity>? EntityRepository { get; set; }
        protected ILogger<BaseEntityService<TEntity>>? _logger { get; set; }

        public BaseEntityService()
        {
        }

        protected BaseEntityService(IAsyncRepository<TEntity> entityRepository, ILogger<BaseEntityService<TEntity>> logger, IValidator<TEntity>? validator = null)
        {
            EntityRepository = entityRepository;
            _validator = validator;
            _logger = logger;
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
                EntityRepository?.Dispose();
            }
        }

        public void EnsureThatRepositoryExists()
        {
            if (EntityRepository == null)
            {
                throw new Exception("Service has no repository");
            }
        }

        public async Task<TEntity?> GetByIdAsync(object id, List<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default)
        {
            EnsureThatRepositoryExists();

            return await EntityRepository!.GetByIdAsync(id, includes, cancellationToken);
        }

        public async Task<IList<TEntity>> ListAllAsync(List<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default)
        {
            EnsureThatRepositoryExists();

            return await EntityRepository!.ListAllAsync(includes, cancellationToken);
        }

        public virtual async Task<QueryResult<TEntity>> QueryAsync(ISpecification<TEntity> specification, bool includeCount = false, CancellationToken cancellationToken = default)
        {
            if (specification is null)
            {
                throw new ArgumentNullException(nameof(specification));
            }

            EnsureThatRepositoryExists();

            specification.BuildQuery();

            return await EntityRepository!.ListAsync(specification, includeCount, cancellationToken);
        }

        public virtual async Task<TEntity> GetFirstByQueryAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            specification.Take = 1;
            specification.Skip = 0;

            return (await QueryAsync(specification, false, cancellationToken)).Results.FirstOrDefault();
        }

        public virtual async Task<QueryResult<TTargetEntity>> QueryWithProjectionAsync<TTargetEntity>(ISpecification<TEntity>? specification, bool includeCount = false, CancellationToken cancellationToken = default)
            where TTargetEntity : class
        {
            if (specification is null)
            {
                throw new ArgumentNullException(nameof(specification));
            }

            EnsureThatRepositoryExists();

            specification.BuildQuery();

            if(specification.Includes?.Any(x => x.Body?.NodeType != ExpressionType.MemberAccess) ?? false)
            {
                _logger?.LogWarning("Query with projection will ignore include and especially function include");
            }                   

            return await EntityRepository!.ListWithProjectionAsync<TTargetEntity>(specification, includeCount, cancellationToken);
        }

        public virtual async Task<TTargetEntity> GetFirstByQueryWithProjectionAsync<TTargetEntity>(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) 
            where TTargetEntity : class
        {
            specification.Take = 1;
            specification.Skip = 0;

            return (await QueryWithProjectionAsync<TTargetEntity>(specification, false, cancellationToken)).Results.FirstOrDefault();
        }

        public virtual async Task<CommandResult<TEntity>> CreateAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            return await ValidateAndProcess(entity, async (x) => await InnerAddAsync(x, cancellationToken: cancellationToken));
        }

        public virtual async Task<ICollection<CommandResult<TEntity>>> CreateRangeAsync(ICollection<TEntity> entities, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            return await ValidateAndProcess(entities, async (x) => await InnerAddRangeAsync(x, cancellationToken));
        }

        public virtual async Task<int> UpdateRangeAsync(Expression<Func<TEntity, bool>> entityPredicate, Expression<Func<TEntity, TEntity>> updateExpression, CancellationToken cancellationToken = default)
        {
            EnsureThatRepositoryExists();

            return await EntityRepository!.UpdateRangeAsync(entityPredicate, updateExpression, cancellationToken);
        }

        public virtual async Task<CommandResult<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return await ValidateAndProcess(entity, async (x) => await InnerUpdateAsync(x, cancellationToken));
        }

        private async Task<CommandResult<TEntity>> ValidateAndProcess(TEntity entity, Func<TEntity, Task<CommandResult<TEntity>>> action)
        {
            if (_validator != null)
            {
                var validationResult = _validator.Validate(entity);

                if (!validationResult.IsValid)
                {
                    string validationErrorMessage = string.Join(",", validationResult.Errors.Select(x => x.ErrorMessage).ToArray());

                    return new CommandResult<TEntity>() { Entity = entity, Result = CommandState.Error, Message = $"Validation has failed :  {validationErrorMessage}" };
                }
            }

            return await action(entity);
        }

        private async Task<ICollection<CommandResult<TEntity>>> ValidateAndProcess(ICollection<TEntity> entities, Func<ICollection<TEntity>, Task<ICollection<CommandResult<TEntity>>>> action)
        {
            List<CommandResult<TEntity>> results = new List<CommandResult<TEntity>>();

            ICollection<TEntity> entitiesToHandle = new List<TEntity>();

            if (_validator != null)
            {
                foreach(var entity in entities)
                {
                    var validationResult = _validator.Validate(entity);

                    if (!validationResult.IsValid)
                    {
                        string validationErrorMessage = string.Join(",", validationResult.Errors.Select(x => x.ErrorMessage).ToArray());

                        results.Add(new CommandResult<TEntity>() { Entity = entity, Result = CommandState.Error, Message = $"Validation has failed :  {validationErrorMessage}" });
                    }
                    else
                    {
                        entitiesToHandle.Add(entity);
                    }
                }
            }
            else
            {
                entitiesToHandle = entities;
            }

            results.AddRange(await action(entitiesToHandle));

            return results;
        }

        private async Task<CommandResult<TEntity>> InnerUpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            EnsureThatRepositoryExists();

            CommandResult<TEntity> result = await EntityRepository!.UpdateAsync(entity, cancellationToken: cancellationToken);

            return new CommandResult<TEntity>() { Entity = entity, Result = result.Result };
        }

        public async Task<CommandResult<TEntity>> InnerAddAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            EnsureThatRepositoryExists();

            TEntity result = await EntityRepository!.AddAsync(entity, saveChanges, cancellationToken);

            return new CommandResult<TEntity>() { Entity = result, Result = CommandState.Success };
        }

        private async Task<ICollection<CommandResult<TEntity>>> InnerAddRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default)
        {
            EnsureThatRepositoryExists();

            await EntityRepository!.AddRangeAsync(entities, cancellationToken: cancellationToken);

            return entities.Select(x => new CommandResult<TEntity>(CommandState.Success) { Entity = x }).ToList();
        }

        //private async Task<ICollection<CommandResult<TEntity>>> InnerUpdateRangeAsync(ICollection<TEntity> entities)
        //{
        //    await EntityRepository.UpdateRangeAsync(entities);

        //    return entities.Select(x => new CommandResult<TEntity>(CommandState.Success) { Entity = x }).ToList();
        //}

        public virtual async Task<CommandResult<TEntity>> UpdatePropertiesAsync(TEntity entity, string[] properties, CancellationToken cancellationToken = default)
        {
            EnsureThatRepositoryExists();

            return await EntityRepository!.UpdatePropertiesAsync(entity, properties, cancellationToken: cancellationToken);
        }

        public virtual async Task<CommandResult> DeleteAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            EnsureThatRepositoryExists();

            return await EntityRepository!.DeleteAsync(entity, saveChanges, cancellationToken);
        }

        public bool Validate(TEntity entity)
        {
            return _validator?.Validate(entity).IsValid ?? throw new ValidatorNotFoundException(typeof(TEntity));
        }
    }
}
