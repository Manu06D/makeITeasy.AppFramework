﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using makeITeasy.CarCatalog.dotnet7.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet7.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace makeITeasy.CarCatalog.dotnet7.Infrastructure.Data.Configurations
{
    public partial class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> entity)
        {
            entity.ToTable("Country");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.CountryCode)
                .IsRequired()
                .HasMaxLength(2)
                .IsFixedLength();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Country> entity);
    }
}
