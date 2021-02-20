namespace makeITeasy.AppFramework.Models
{
    public abstract class BaseTransactionQuery<T> : BaseQuery<T>, ITransactionSpecification<T> where T : IBaseEntity
    {
        public bool ReadDirty { get; set; }
    }
}
