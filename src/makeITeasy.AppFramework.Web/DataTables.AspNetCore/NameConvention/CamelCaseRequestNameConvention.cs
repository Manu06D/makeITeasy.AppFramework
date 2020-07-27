using System;
using System.Collections.Generic;
using System.Text;

namespace makeITeasy.AppFramework.Web.DataTables.AspNetCore.NameConvention
{
    public interface IRequestNameConvention
    {
        string Draw { get; }
        string Start { get; }
        string Length { get; }
        string SearchValue { get; }
        string IsSearchRegex { get; }
        string SortField { get; }
        string SortDirection { get; }
        string ColumnField { get; }
        string ColumnName { get; }
        string IsColumnSearchable { get; }
        string IsColumnSortable { get; }
        string ColumnSearchValue { get; }
        string IsColumnSearchRegex { get; }
        string SortAscending { get; }
        string SortDescending { get; }
    }

    public class CamelCaseRequestNameConvention : IRequestNameConvention
    {
        public string Draw { get { return "draw"; } }
        public string Start { get { return "start"; } }
        public string Length { get { return "length"; } }
        public string SearchValue { get { return "search[value]"; } }
        public string IsSearchRegex { get { return "search[regex]"; } }
        public string SortField { get { return "order[{0}][column]"; } }
        public string SortDirection { get { return "order[{0}][dir]"; } }
        public string ColumnField { get { return "columns[{0}][data]"; } }
        public string ColumnName { get { return "columns[{0}][name]"; } }
        public string IsColumnSearchable { get { return "columns[{0}][searchable]"; } }
        public string IsColumnSortable { get { return "columns[{0}][orderable]"; } }
        public string ColumnSearchValue { get { return "columns[{0}][search][value]"; } }
        public string IsColumnSearchRegex { get { return "columns[{0}][search][regex]"; } }
        public string SortAscending { get { return "asc"; } }
        public string SortDescending { get { return "desc"; } }
    }
}
