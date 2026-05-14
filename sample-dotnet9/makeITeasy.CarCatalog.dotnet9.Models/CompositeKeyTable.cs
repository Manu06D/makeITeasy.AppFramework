using System;
using System.Collections.Generic;
using makeITeasy.AppFramework.Models;

namespace makeITeasy.CarCatalog.dotnet9.Models
{
    public partial class CompositeKeyTable: IBaseEntity
    {
        public CompositeKeyTable()
        {
            CompositeKeySubTables = new HashSet<CompositeKeySubTable>();
        }

        public int Id1 { get; set; }
        public int Id2 { get; set; }
        public int? Col1 { get; set; }

        public virtual ICollection<CompositeKeySubTable> CompositeKeySubTables { get; set; }
    }
}
