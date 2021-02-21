using System.Transactions;

namespace makeITeasy.AppFramework.Models
{
    public abstract class BaseTransactionQuery<T> : BaseQuery<T>, ITransactionSpecification<T> where T : IBaseEntity
    {
        public IsolationLevel? IsolationLevel { get; set; }
    }
}
