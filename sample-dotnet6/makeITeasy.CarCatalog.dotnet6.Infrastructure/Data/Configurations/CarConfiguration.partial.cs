﻿using makeITeasy.CarCatalog.dotnet6.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace makeITeasy.CarCatalog.dotnet6.Infrastructure.Data.Configurations
{
    public partial class CarConfiguration
    {
        partial void OnConfigurePartial(EntityTypeBuilder<Car> entity)
        {
            entity.Property(c => c.Version)
            .HasDefaultValue(0)
            .IsRowVersion();
        }
    }
}
