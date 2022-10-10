using System;
using System.Collections.Generic;
using System.Text;

namespace makeITeasy.AppFramework.Web.DataTables.AspNetCore
{
    public interface IColumn
    {
        string Name { get; }
        string Field { get; }
        ISearch? Search { get; }
        bool IsSearchable { get; }
        ISort? Sort { get; }
        bool IsSortable { get; }

        bool SetSort(int order, string direction);
    }

    public class Column : IColumn
    {
        public string Field { get; private set; }
        public string Name { get; private set; }
        public ISearch? Search { get; private set; }
        public bool IsSearchable { get; private set; }
        public ISort? Sort { get; private set; }
        public bool IsSortable { get; private set; }


        public Column(string name, string field, bool searchable, bool sortable, ISearch search)
        {
            Name = name;
            Field = field;
            IsSortable = sortable;

            IsSearchable = searchable;
            Search = !IsSearchable ? null : search ?? new Search();
        }


        public bool SetSort(int order, string direction)
        {
            if (!IsSortable)
            {
                return false;
            }

            Sort = new Sort(order, direction);
            return true;
        }
    }
}
