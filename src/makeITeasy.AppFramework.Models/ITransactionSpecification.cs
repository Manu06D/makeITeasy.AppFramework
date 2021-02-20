namespace makeITeasy.AppFramework.Models
{
    public interface ITransactionSpecification<T> : ISpecification<T>
    {
        bool ReadDirty { get; set; }
    }
}
