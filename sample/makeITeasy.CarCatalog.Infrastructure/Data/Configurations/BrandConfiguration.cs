﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System;

#nullable disable

namespace makeITeasy.CarCatalog.Infrastructure.Data.Configurations
{
    public partial class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> entity)
        {
            entity.ToTable("Brand");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.CountryId).HasColumnName("CountryID");

            entity.Property(e => e.Logo).HasMaxLength(250);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.Country)
                .WithMany(p => p.Brands)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Brand_ToCountry");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Brand> entity);
    }
}