
namespace makeITeasy.AppFramework.Models
{
    public class OrderBySpecification<T>
    {
        public OrderBySpecification(T orderBy, bool sortDescending = false)
        {
            SortDescending = sortDescending;
            OrderBy = orderBy;
        }

        public bool Any() => OrderBy != null;
        public T OrderBy { get; set; }
        public bool SortDescending { get; set; }
    }
}
