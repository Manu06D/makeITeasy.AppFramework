﻿using System;
using System.Collections.Generic;
using makeITeasy.AppFramework.Models;

namespace makeITeasy.CarCatalog.dotnet9.Models
{
    public partial class Brand: IBaseEntity
    {
        public Brand()
        {
            Cars = new HashSet<Car>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Logo { get; set; }
        public int CountryId { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModificationDate { get; set; }
        public string? DynamicBrandDetails { get; set; }

        public virtual Country Country { get; set; } = null!;
        public virtual ICollection<Car> Cars { get; set; }
    }
}
