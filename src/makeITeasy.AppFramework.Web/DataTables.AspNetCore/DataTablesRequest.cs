﻿using System;
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

        public BaseQuery<U> GetSearchInformation<T,U>() where T : IDatatableBaseConfiguration where U:BaseEntity
        {
            Type searchType = typeof(T).BaseType.GetGenericArguments()[0];

            var searchResult = createObjectWithDefaultConstructor(searchType);

            if (searchResult != null && IsTypeImplementInterface(searchType, typeof(IBaseQuery)))
            {
                if (AdditionalParameters != null && AdditionalParameters.Count > 0)
                {
                    searchResult = AdditionalParameters.ToObject(searchResult);
                }

                ((IBaseQuery)searchResult).IsPagingEnabled = true;
                ((IBaseQuery)searchResult).Skip = Start;
                ((IBaseQuery)searchResult).Take = Length;
                ((IBaseQuery)searchResult).SortBy = SortInformation.Item1;
                ((IBaseQuery)searchResult).SortDescending = !SortInformation.Item2;
            }

            var instance = createObjectWithDefaultConstructor(typeof(T)) as IDatatableBaseConfiguration;

            var filterColum = instance.Columns.Where(x => String.Compare(((IBaseQuery)searchResult).SortBy, x.Name, true) == 0 && !String.IsNullOrEmpty(x.SortDataSource)).FirstOrDefault();

            if (filterColum != null)
            {
                ((IBaseQuery)searchResult).SortBy = filterColum.SortDataSource;
            }

            return ((IBaseQuery)searchResult) as BaseQuery<U>;
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
                        p.HasDefaultValue ? p.DefaultValue : p.ParameterType.IsValueType && Nullable.GetUnderlyingType(p.ParameterType) == null ? Activator.CreateInstance(p.ParameterType) : null
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
