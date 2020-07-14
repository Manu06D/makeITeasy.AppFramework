using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using makeITeasy.AppFramework.Core.Queries;

namespace makeITeasy.AppFramework.Core.Services
{
    public class BaseEntityService<TEntity> : IBaseEntityService<TEntity>, IDisposable where TEntity : class, IBaseEntity
    {
        protected IValidatorFactory ValidatorFactory { get; set; }
        public IAsyncRepository<TEntity> EntityRepository { get; set; }

        public BaseEntityService()
        {
        }

        protected BaseEntityService(IAsyncRepository<TEntity> entityRepository)
        {
            EntityRepository = entityRepository;
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

        public async Task<TEntity> GetByID(object id, List<Expression<Func<TEntity, object>>> includes)
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

        public async Task<CommandResult<TEntity>> Create(TEntity entity, bool saveChanges = true)
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

        //TODO : Add primary attribute to retrieve key property and check if its value is default
        //public async Task<CommandResult<TEntity>> CreateOrUpdate(TEntity entity)
        //{
        //    
        //    if (entity?.ID <= 0)
        //    {
        //        return await Add(entity);
        //    }

        //    return await UpdateAsync(entity);
        //}
    }
}
