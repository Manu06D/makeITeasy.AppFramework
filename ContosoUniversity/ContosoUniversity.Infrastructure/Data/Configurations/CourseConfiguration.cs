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
    public partial class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> entity)
        {
            entity.ToTable("Course");

            entity.Property(e => e.CourseId)
                .ValueGeneratedNever()
                .HasColumnName("CourseID");

            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Department)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.DepartmentId);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Course> entity);
    }
}
