using System;
using System.Collections.Generic;
using makeITeasy.AppFramework.Models;

namespace makeITeasy.CarCatalog.dotnet9.Models
{
    public partial class Engine: IBaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
