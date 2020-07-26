﻿using System.Collections.Generic;
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

        BaseQuery<U> GetSearchInformation<T, U>() where T : IDatatableBaseConfiguration where U : BaseEntity;
    }
}
