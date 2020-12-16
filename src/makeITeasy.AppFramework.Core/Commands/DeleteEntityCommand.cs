using makeITeasy.AppFramework.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace makeITeasy.AppFramework.Core.Commands
{
    public class DeleteEntityCommand<TEntity> : IRequest<CommandResult<TEntity>> where TEntity : IBaseEntity
    {
        public TEntity Entity { get; private set; }

        public DeleteEntityCommand(TEntity entity)
        {
            this.Entity = entity;
        }
    }
}
