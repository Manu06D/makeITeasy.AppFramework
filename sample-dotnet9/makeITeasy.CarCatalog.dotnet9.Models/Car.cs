﻿using System;
using System.Collections.Generic;
using makeITeasy.AppFramework.Models;

namespace makeITeasy.CarCatalog.dotnet9.Models
{
    public partial class Car: IBaseEntity
    {
        public Car()
        {
            CarDetails = new HashSet<CarDetail>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public int BrandId { get; set; }
        public int ReleaseYear { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModificationDate { get; set; }
        public byte[]? RowVersion { get; set; }
        //public int Version { get; set; }

        public virtual Brand Brand { get; set; } = null!;
        public virtual ICollection<CarDetail> CarDetails { get; set; }
    }
}
