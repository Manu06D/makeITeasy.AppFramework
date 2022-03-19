﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using ContosoUniversity.Infrastructure;
using ContosoUniversity.Infrastructure.Data;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace ContosoUniversity.Infrastructure.Data.Configurations
{
    public partial class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> entity)
        {
            entity.ToTable("Department");

            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

            entity.Property(e => e.Budget).HasColumnType("money");

            entity.Property(e => e.InstructorId).HasColumnName("InstructorID");

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Instructor)
                .WithMany(p => p.Departments)
                .HasForeignKey(d => d.InstructorId);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Department> entity);
    }
}
