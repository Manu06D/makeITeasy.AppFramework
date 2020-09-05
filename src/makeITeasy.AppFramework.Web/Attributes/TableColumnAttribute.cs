using System;

namespace makeITeasy.AppFramework.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class TableColumnAttribute : Attribute
    {
        public string SortDataSource { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public bool Sortable { get; set; } = true;
        public bool AutoWidth { get; set; } = true;
        public bool Visible { get; set; } = true;
        public int Priority { get; set; } = int.MaxValue;
        public bool IsRowId { get; set; } = false;
        public int Order { get; set; }
    }
}
