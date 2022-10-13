using System.Collections.Generic;
using makeITeasy.AppFramework.Models;
using makeITeasy.AppFramework.Web.Models;

namespace makeITeasy.AppFramework.Web.DataTables.AspNetCore
{
    public interface IDataTablesRequest
    {
        int Draw { get; }
        int Start { get; }
        int Length { get; }
        ISearch Search { get; }
        IEnumerable<IColumn> Columns { get; }
        IDictionary<string, object> AdditionalParameters { get; }
        (string, bool) SortInformation { get; }
        List<(string, bool)> SortsInformation { get; }

        V GetSearchInformation<T, U, V>() where T : IDatatableBaseConfiguration where U : IBaseEntity where V : ISpecification<U>;
    }
}
