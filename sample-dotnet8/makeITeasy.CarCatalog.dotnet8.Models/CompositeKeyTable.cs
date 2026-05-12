using System;
using System.Collections.Generic;
using makeITeasy.AppFramework.Models;

namespace makeITeasy.CarCatalog.dotnet8.Models
{
    public partial class CompositeKeyTable: IBaseEntity
    {
public int Id1 { get; set; }
public int Id2 { get; set; }
public int? Col1 { get; set; }
    }
}
