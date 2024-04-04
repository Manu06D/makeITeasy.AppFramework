using System;
using System.Collections.Generic;
using makeITeasy.AppFramework.Models;

namespace makeITeasy.CarCatalog.dotnet8.Models
{
    public partial class Country: IBaseEntity
    {
public Country()
{
    Brands = new HashSet<Brand>();
 }

public int Id { get; set; }
public string CountryCode { get; set; } = default!;
public string Name { get; set; } = default!;
public DateTime? CreationDate { get; set; }
public DateTime? LastModificationDate { get; set; }

public virtual ICollection<Brand> Brands { get; set; } = default!;
    }
}
