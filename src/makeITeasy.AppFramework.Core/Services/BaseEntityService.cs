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

namespace makeITeasy.AppFramework.Core.Services
{
    public class BaseEntityService<TEntity> : IBaseEntityService<TEntity>, IDisposable where TEntity : class, IBaseEntity
    {
        protected IValidatorFactory ValidatorFactory { get; set; }
        public IAsyncRepository<TEntity> EntityRepository { get; set; }
        protected ILogger<BaseEntityService<TEntity>> _logger { get; set; }

        public BaseEntityService()
        {
        }

        protected BaseEntityService(IAsyncRepository<TEntity> entityRepository, IValidatorFactory validatorFactory, ILogger<BaseEntityService<TEntity>> logger)
        {
            EntityRepository = entityRepository;
            ValidatorFactory = validatorFactory;
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

        public async Task<TEntity> GetByIdAsync(object id, List<Expression<Func<TEntity, object>>> includes = null)
        {
            return await EntityRepository.GetByIdAsync(id, includes);
        }

        public async Task<QueryResult<TEntity>> QueryAsync(ISpecification<TEntity> specification, bool includeCount = false)
        {
            specification?.BuildQuery();

            return await EntityRepository.ListAsync(specification, includeCount);
        }

        public async Task<QueryResult<TTargetEntity>> QueryWithProjectionAsync<TTargetEntity>(ISpecification<TEntity> specification, bool includeCount = false)
            where TTargetEntity : class
        {
            specification?.BuildQuery();

            if(specification?.Includes?.Any(x => x.Body?.NodeType != ExpressionType.MemberAccess) ?? false)
            {
                _logger?.LogWarning("Query with projection will ignore include and especially function include");
            }                   

            return await EntityRepository.ListWithProjectionAsync<TTargetEntity>(specification, includeCount);
        }

        public virtual async Task<CommandResult<TEntity>> CreateAsync(TEntity entity, bool saveChanges = true)
        {
            return await ValidateAndProcess(entity, async (x) => await InnerAddAsync(x));
        }

        public virtual async Task<ICollection<CommandResult<TEntity>>> CreateRangeAsync(ICollection<TEntity> entities, bool saveChanges = true)
        {
            return await ValidateAndProcess(entities, async (x) => await InnerAddRangeAsync(x));
        }

        public virtual async Task<int> UpdateRangeAsync(Expression<Func<TEntity, bool>> entityPredicate, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            return await EntityRepository.UpdateRangeAsync(entityPredicate, updateExpression);
        }

        public async Task<CommandResult<TEntity>> UpdateAsync(TEntity entity)
        {
            return await ValidateAndProcess(entity, async (x) => await InnerUpdateAsync(x));
        }

        private async Task<CommandResult<TEntity>> ValidateAndProcess(TEntity entity, Func<TEntity, Task<CommandResult<TEntity>>> action)
        {
            var entityValidator = ValidatorFactory?.GetValidator<TEntity>();

            if (entityValidator != null)
            {
                var validationResult = entityValidator.Validate(entity);

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
            var entityValidator = ValidatorFactory?.GetValidator<TEntity>();

            List<CommandResult<TEntity>> results = new List<CommandResult<TEntity>>();

            ICollection<TEntity> entitiesToHandle = new List<TEntity>();

            if (entityValidator != null)
            {
                foreach(var entity in entities)
                {
                    var validationResult = entityValidator.Validate(entity);

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

        private async Task<CommandResult<TEntity>> InnerUpdateAsync(TEntity entity)
        {
            await EntityRepository.UpdateAsync(entity);

            return new CommandResult<TEntity>() { Entity = entity, Result = CommandState.Success };
        }

        public async Task<CommandResult<TEntity>> InnerAddAsync(TEntity entity, bool saveChanges = true)
        {
            TEntity result = await EntityRepository.AddAsync(entity, saveChanges);

            return new CommandResult<TEntity>() { Entity = result, Result = CommandState.Success };
        }

        private async Task<ICollection<CommandResult<TEntity>>> InnerAddRangeAsync(ICollection<TEntity> entities)
        {
            await EntityRepository.AddRangeAsync(entities);

            return entities.Select(x => new CommandResult<TEntity>(CommandState.Success) { Entity = x}).ToList();
        }

        //private async Task<ICollection<CommandResult<TEntity>>> InnerUpdateRangeAsync(ICollection<TEntity> entities)
        //{
        //    await EntityRepository.UpdateRangeAsync(entities);

        //    return entities.Select(x => new CommandResult<TEntity>(CommandState.Success) { Entity = x }).ToList();
        //}

        public async Task<CommandResult<TEntity>> UpdatePropertiesAsync(TEntity entity, string[] properties)
        {
            return await EntityRepository.UpdatePropertiesAsync(entity, properties);
        }

        public async Task<CommandResult> DeleteAsync(TEntity entity, bool saveChanges = true)
        {
            return await EntityRepository.DeleteAsync(entity, saveChanges);
        }

        public bool Validate(TEntity entity)
        {
            return ValidatorFactory.GetValidator<TEntity>()?.Validate(entity).IsValid ?? throw new ValidatorNotFoundException(typeof(TEntity));
        }
    }
}
