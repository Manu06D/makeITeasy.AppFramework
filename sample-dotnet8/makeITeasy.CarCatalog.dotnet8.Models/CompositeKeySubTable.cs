using System;
using System.Collections.Generic;
using makeITeasy.AppFramework.Models;

namespace makeITeasy.CarCatalog.dotnet8.Models
{
    public partial class CompositeKeySubTable: IBaseEntity
    {
public int Id { get; set; }
public int ParentKey1 { get; set; }
public int ParentKey2 { get; set; }
public string Value { get; set; } = default!;

public virtual CompositeKeyTable CompositeKeyTable { get; set; } = default!;
    }
}
