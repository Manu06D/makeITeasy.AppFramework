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

namespace makeITeasy.AppFramework.Core.Services
{
    public class BaseEntityService<TEntity> : IBaseEntityService<TEntity>, IDisposable where TEntity : class, IBaseEntity
    {
        protected IValidatorFactory ValidatorFactory { get; set; }
        public IAsyncRepository<TEntity> EntityRepository { get; set; }

        public BaseEntityService()
        {
        }

        protected BaseEntityService(IAsyncRepository<TEntity> entityRepository, IValidatorFactory validatorFactory)
        {
            EntityRepository = entityRepository;
            ValidatorFactory = validatorFactory;
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
                EntityRepository.Dispose();
            }
        }

        public async Task<TEntity> GetByIdAsync(object id, List<Expression<Func<TEntity, object>>> includes = null)
        {
            return await EntityRepository.GetByIdAsync(id, includes);
        }

        public async Task<QueryResult<TEntity>> QueryAsync(BaseQuery<TEntity> specification, bool includeCount = false)
        {
            specification?.BuildQuery();

            return await EntityRepository.ListAsync(specification, includeCount);
        }

        public async Task<QueryResult<TTargetEntity>> QueryWithProjectionAsync<TTargetEntity>(BaseQuery<TEntity> specification, bool includeCount = false)
            where TTargetEntity : class
        {
            specification?.BuildQuery();

            return await EntityRepository.ListWithProjectionAsync<TTargetEntity>(specification, includeCount);
        }

        public virtual async Task<CommandResult<TEntity>> CreateAsync(TEntity entity, bool saveChanges = true)
        {
            return await SaveOrUpdate(entity, async (x) => await InnerAddAsync(x));
        }

        public async Task<CommandResult<TEntity>> UpdateAsync(TEntity entity)
        {
            return await SaveOrUpdate(entity, async (x) => await InnerUpdateAsync(x));
        }

        private async Task<CommandResult<TEntity>> SaveOrUpdate(TEntity entity, Func<TEntity, Task<CommandResult<TEntity>>> action)
        {
            var entityValidator = ValidatorFactory?.GetValidator<TEntity>();

            if (entityValidator != null)
            {
                var validationResult = entityValidator.Validate(entity);

                if (!validationResult.IsValid)
                {
                    string validationErrorMessage = String.Join(",", validationResult.Errors.Select(x => x.ErrorMessage).ToArray());
                    return new CommandResult<TEntity>() { Entity = entity, Result = CommandState.Error, Message = $"Validation has failed :  {validationErrorMessage}" };
                }
            }

            return await action(entity);
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

        public async Task<CommandResult<TEntity>> UpdatePropertiesAsync(TEntity entity, string[] properties)
        {
            return await EntityRepository.UpdatePropertiesAsync(entity, properties);
        }

        public async Task<int> DeleteAsync(TEntity entity, bool saveChanges = true)
        {
            return await EntityRepository.DeleteAsync(entity, saveChanges);
        }

        public bool Validate(TEntity entity)
        {
            return ValidatorFactory.GetValidator<TEntity>()?.Validate(entity).IsValid ?? throw new ValidatorNotFoundException(typeof(TEntity));
        }
    }
}
