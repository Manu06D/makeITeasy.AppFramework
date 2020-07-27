using System;
using System.Collections.Generic;
using System.Text;

namespace makeITeasy.AppFramework.Web.Models
{
    public class DatatableColumnBase
    {
        public string SortDataSource { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public bool AutoWidth { get; set; } = true;

        public bool Visible { get; set; } = true;

        public bool Orderable { get; set; } = true;

        public int Priority { get; set; } = int.MaxValue;

        public bool IsRowId { get; set; } = false;

        public int Order { get; set; } = 0;
    }
}
