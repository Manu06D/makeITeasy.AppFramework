﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace makeITeasy.CarCatalog.Infrastructure.Data.Configurations
{
    public partial class CarDetailConfiguration : IEntityTypeConfiguration<CarDetail>
    {
        public void Configure(EntityTypeBuilder<CarDetail> entity)
        {
            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.CarId).HasColumnName("CarID");

            //entity.Property(e => e.DynamicCarDetails).IsRequired();

            entity.HasOne(d => d.Car)
                .WithMany(p => p.CarDetails)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CarDetails_ToCar");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<CarDetail> entity);
    }
}