using System.Collections.Generic;

namespace makeITeasy.AppFramework.Web.Models
{
    public interface IDatatableBaseConfiguration
    {
        string ApiUrl { get; }
        List<DatatableColumnBase> Columns { get; }
        DatatableOptions Options { get; }
        long TableID { get; }
    }
}
