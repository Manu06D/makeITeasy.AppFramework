using System.Transactions;

namespace makeITeasy.AppFramework.Models
{
    public interface ITransactionSpecification<T> : ISpecification<T>
    {
        IsolationLevel? IsolationLevel{ get; set; }
    }
}
