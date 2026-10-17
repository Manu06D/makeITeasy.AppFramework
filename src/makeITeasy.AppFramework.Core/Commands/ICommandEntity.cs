namespace makeITeasy.AppFramework.Core.Commands
{
    public interface ICommandEntity<TEntity> where TEntity : IBaseEntity
    {
        TEntity Entity { get; }
    }
}
