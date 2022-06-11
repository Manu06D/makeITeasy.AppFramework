namespace makeITeasy.AppFramework.Models
{
    public abstract class ReadUncommitedQuery<T> : BaseTransactionQuery<T> where T : IBaseEntity
    {
        public ReadUncommitedQuery()
        {
            IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
        }
    }
}
