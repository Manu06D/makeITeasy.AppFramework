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
    public partial class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> entity)
        {
            entity.ToTable("Student");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Student> entity);
    }
}
