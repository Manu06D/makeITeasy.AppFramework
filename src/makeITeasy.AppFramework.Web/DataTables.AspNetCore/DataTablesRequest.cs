using System;
using System.Collections.Generic;
using System.Linq;
using makeITeasy.AppFramework.Models;
using makeITeasy.AppFramework.Web.Models;

namespace makeITeasy.AppFramework.Web.DataTables.AspNetCore
{
    public class DataTablesRequest : IDataTablesRequest
    {
        public IDictionary<string, object> AdditionalParameters { get; private set; }
        public IEnumerable<IColumn> Columns { get; private set; }
        public int Draw { get; private set; }
        public int Length { get; private set; }
        public ISearch Search { get; private set; }
        public int Start { get; private set; }

        public (string, bool) SortInformation
        {
            get
            {
                string orderColumn = string.Empty;
                bool sortAscending = true;

                IColumn? sortColumn = Columns?.FirstOrDefault(x => x.Sort != null);

                if (sortColumn != null)
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

        public DataTablesRequest(int draw, int start, int length, ISearch search, IEnumerable<IColumn> columns, IDictionary<string, object> additionalParameters)
        {
            Draw = draw;
            Start = start;
            Length = length;
            Search = search;
            Columns = columns;
            AdditionalParameters = additionalParameters;
        }

        public BaseQuery<U> GetSearchInformation<T,U>() where T : IDatatableBaseConfiguration where U:IBaseEntity
        {
            Type searchType = typeof(T).BaseType.GetGenericArguments()[0];

            var searchResult = createObjectWithDefaultConstructor(searchType);

            var orderBySpecification = new OrderBySpecification<string>(SortInformation.Item1, !SortInformation.Item2);

            if (searchResult != null && IsTypeImplementInterface(searchType, typeof(ISpecification<U>)))
            {
                if (AdditionalParameters != null && AdditionalParameters.Count > 0)
                {
                    searchResult = AdditionalParameters.ToObject(searchResult);
                }

                ((ISpecification<U>)searchResult).Skip = Start;
                ((ISpecification<U>)searchResult).Take = Length;
                
                ((ISpecification<U>)searchResult).OrderByStrings = new List<OrderBySpecification<string>>() {orderBySpecification};
            }

            var instance = createObjectWithDefaultConstructor(typeof(T)) as IDatatableBaseConfiguration;

            var filterColum = instance.Columns.FirstOrDefault(x => string.Compare(orderBySpecification.OrderBy, x.Name, true) == 0  && !string.IsNullOrEmpty(x.SortDataSource));

            if (filterColum != null)
            {
                orderBySpecification.OrderBy = filterColum.SortDataSource;
            }

            return ((ISpecification<U>)searchResult) as BaseQuery<U>;
        }

        private bool IsTypeImplementInterface(Type type, Type searchInterface)
        {
            return type.GetInterfaces().Any(x => x == searchInterface);
        }

        private static object createObjectWithDefaultConstructor(Type type)
        {
            object instance = null;

            var datatableCtr = type.GetConstructors().OrderBy(x => x.GetParameters().Length).FirstOrDefault();

            if (datatableCtr != null)
            {
                object[] parameters =
                    datatableCtr.GetParameters().Select(p =>
                        p.HasDefaultValue ? p.DefaultValue : 
                        p.ParameterType.IsValueType && Nullable.GetUnderlyingType(p.ParameterType) == null ? Activator.CreateInstance(p.ParameterType) : null
                    ).ToArray();

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
