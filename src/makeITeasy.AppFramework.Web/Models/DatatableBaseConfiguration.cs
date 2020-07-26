﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using makeITeasy.AppFramework.Web.Attributes;
using makeITeasy.AppFramework.Web.Helpers;

namespace makeITeasy.AppFramework.Web.Models
{
    public class DatatableBaseConfiguration<TSearch, TResult> : IDatatableBaseConfiguration where TSearch : class where TResult : class
    {
        public TSearch SearchModel = default(TSearch);

        [JsonPropertyName("options")]
        public DatatableOptions Options { get; } = new DatatableOptions();

        public List<DatatableColumnBase> Columns { get; private set; } = new List<DatatableColumnBase>();

        public long TableID { get; } = DateTime.Now.Ticks;

        public String? ApiUrl { get; }

        public DatatableBaseConfiguration(String apiUrl)
        {
            ApiUrl = apiUrl;

            parseColumnProperties();
        }

        private void parseColumnProperties()
        {
            var attributesProperties = retieveAttributesFromType(typeof(TResult));

            int counter = 0;
            foreach (string propertyName in attributesProperties.Keys)
            {
                var t = new DatatableColumnBase();

                t.SortDataSource = attributesProperties[propertyName].SortDataSource;
                t.Name = attributesProperties[propertyName].Name.ToCamelCase();
                t.Title = attributesProperties[propertyName].Title;
                t.AutoWidth = attributesProperties[propertyName].AutoWidth;
                t.Orderable = attributesProperties[propertyName].Sortable;
                t.Priority = attributesProperties[propertyName].Priority;
                t.Visible = attributesProperties[propertyName].Visible;
                t.IsRowId = attributesProperties[propertyName].IsRowId;
                t.Order = attributesProperties[propertyName].Order > 0 ? attributesProperties[propertyName].Order : counter;

                Columns.Add(t);
                counter++;
            }

            Columns = Columns.OrderBy(t => t.Order).ToList();
        }

        private static Dictionary<string, TableColumnAttribute> retieveAttributesFromType(Type type)
        {
            var attributesProperty = new Dictionary<string, TableColumnAttribute>();

            foreach (var property in type.GetProperties())
            {
                TableColumnAttribute? attribute = property.GetCustomAttribute<TableColumnAttribute>();

                if (attribute != null)
                {
                    attributesProperty[property.Name] = attribute;
                }
            }

            return attributesProperty;
        }
    }
}
