using System.Collections.Generic;

namespace makeITeasy.AppFramework.Core.Queries
{
    public class QueryResult<T> where T : class
    {
        public IList<T> Results { get; set; } = new List<T>();
        public int TotalItems { get; set; }
    }
}
