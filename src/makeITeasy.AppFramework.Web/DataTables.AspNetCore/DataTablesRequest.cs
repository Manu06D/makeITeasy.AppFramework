using System;
using System.Collections.Generic;
using System.Linq;
using makeITeasy.AppFramework.Models;
using makeITeasy.AppFramework.Web.Models;

namespace makeITeasy.AppFramework.Web.DataTables.AspNetCore
{
    public class DataTablesRequest : IDataTablesRequest
    {
        public IDictionary<string, object>? AdditionalParameters { get; private set; }
        public IEnumerable<IColumn> Columns { get; private set; }
        public int Draw { get; private set; }
        public int Length { get; private set; }
        public ISearch Search { get; private set; }
        public int Start { get; private set; }

        public List<(string, bool)>? SortsInformation
        {
            get
            {
                if(Columns == null)
                {
                    return null;
                }

                return
                    Columns.Where(x => x.Sort != null).ToList()
                        .ConvertAll(y => (y.Field, y.Sort?.Direction == SortDirection.Ascending));
            }
        }

        public (string, bool) SortInformation
        {
            get
            {
                string orderColumn = string.Empty;
                bool sortAscending = true;

                IColumn? sortColumn = Columns?.FirstOrDefault(x => x.Sort != null);

                if (sortColumn?.Sort != null)
                {
                    orderColumn = sortColumn.Field;
                    sortAscending = sortColumn.Sort.Direction == SortDirection.Ascending;
                }

                return (orderColumn, sortAscending);
            }
        }

        public DataTablesRequest(int draw, int start, int length, ISearch search, IEnumerable<IColumn> columns)
            : this(draw, start, length, search, columns, null)
        {
        }

        public DataTablesRequest(int draw, int start, int length, ISearch search, IEnumerable<IColumn> columns, IDictionary<string, object>? additionalParameters)
        {
            Draw = draw;
            Start = start;
            Length = length;
            Search = search;
            Columns = columns;
            AdditionalParameters = additionalParameters;
        }

        public V? GetSearchInformation<T, U, V>() where T : IDatatableBaseConfiguration where U : IBaseEntity where V : ISpecification<U>
        {
            Type? searchType = typeof(T).BaseType?.GetGenericArguments()[0];

            if(searchType == null)
            {
                return default;
            }

            object? searchResult = createObjectWithDefaultConstructor(searchType);

            List<OrderBySpecification<string>> sortOrders = SortsInformation?.ConvertAll(x => new OrderBySpecification<string>(x.Item1, !x.Item2)) ?? new List<OrderBySpecification<string>>() { };

            if (searchResult != null && IsTypeImplementInterface(searchType, typeof(ISpecification<U>)))
            {
                if (AdditionalParameters != null && AdditionalParameters.Count > 0)
                {
                    searchResult = AdditionalParameters.ToObject(searchResult);
                }

                ((ISpecification<U>)searchResult).Skip = Start;
                ((ISpecification<U>)searchResult).Take = Length;

                ((ISpecification<U>)searchResult).OrderByStrings = sortOrders;
            }

            var instance = createObjectWithDefaultConstructor(typeof(T)) as IDatatableBaseConfiguration;

            foreach (var sortOrder in sortOrders)
            {
                var filterColum = instance?.Columns.FirstOrDefault(x => string.Compare(sortOrder.OrderBy, x.Name, true) == 0 && !string.IsNullOrEmpty(x.SortDataSource));

                if (filterColum != null)
                {
                    sortOrder.OrderBy = filterColum.SortDataSource;
                }
            }

            return (V?)searchResult;
        }

        private static bool IsTypeImplementInterface(Type type, Type searchInterface)
        {
            return type.GetInterfaces().Any(x => x == searchInterface);
        }

        private static object? createObjectWithDefaultConstructor(Type type)
        {
            object? instance;

            var datatableCtr = type.GetConstructors().OrderBy(x => x.GetParameters().Length).FirstOrDefault();

            if (datatableCtr != null)
            {
                object?[] parameters =
                    datatableCtr.GetParameters()?.Select(p =>
                        p.HasDefaultValue ? p.DefaultValue : 
                        p.ParameterType.IsValueType && Nullable.GetUnderlyingType(p.ParameterType) == null ? Activator.CreateInstance(p.ParameterType) : null
                    ).ToArray() ?? Array.Empty<object>();

                instance = datatableCtr.Invoke(parameters);
            }
            else
            {
                instance = Activator.CreateInstance(type);
            }

            return instance;
        }
    }
}
