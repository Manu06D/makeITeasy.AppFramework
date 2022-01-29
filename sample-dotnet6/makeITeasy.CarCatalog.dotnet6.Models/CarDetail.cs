using System;
using System.Collections.Generic;
using makeITeasy.AppFramework.Models;

namespace makeITeasy.CarCatalog.dotnet6.Models
{
    public partial class CarDetail: IBaseEntity
    {
public long Id { get; set; }
public long CarId { get; set; }

public virtual Car Car { get; set; }
    }
}
