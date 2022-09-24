﻿using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.Models.DynamicModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using System.Text.Json;

namespace makeITeasy.CarCatalog.Infrastructure.Data.Configurations
{
    public partial class CarDetailConfiguration
    {
        partial void OnConfigurePartial(EntityTypeBuilder<CarDetail> entity)
        {
            entity.Property(e => e.CarDetails).IsRequired()
                .HasConversion<CarDetailsConverter>();
            ;
        }

    }

    public class CarDetailsConverter : ValueConverter<CarDetailsModel, string>
    {
        public CarDetailsConverter()
            : base(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<CarDetailsModel>(v, (JsonSerializerOptions)null))
        {
        }
    }
}