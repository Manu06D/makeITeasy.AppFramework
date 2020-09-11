
namespace makeITeasy.AppFramework.Models
{
    public class OrderBySpecification<T>
    {
        public bool Any() => OrderBy != null;
        public T OrderBy { get; set; }
        public bool SortDescending { get; set; }
    }
}
