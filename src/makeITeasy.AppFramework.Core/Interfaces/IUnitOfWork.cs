namespace makeITeasy.AppFramework.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> CommitAsync();
        IAsyncRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IBaseEntity;
    }
}
