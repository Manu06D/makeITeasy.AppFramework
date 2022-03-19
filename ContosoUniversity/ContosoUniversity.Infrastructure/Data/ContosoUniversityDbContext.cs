
using ContosoUniversity.Infrastructure;
using ContosoUniversity.Infrastructure.Data.Configurations;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
namespace ContosoUniversity.Infrastructure.Data
{
    public partial class ContosoUniversityDbContext : DbContext
    {
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<CourseAssignment> CourseAssignments { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Enrollment> Enrollments { get; set; }
        public virtual DbSet<Instructor> Instructors { get; set; }
        public virtual DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public virtual DbSet<Student> Students { get; set; }

        public ContosoUniversityDbContext(DbContextOptions<ContosoUniversityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Scaffolding:ConnectionString", "Data Source=(local);Initial Catalog=ContosoUniversity.Databases;Integrated Security=true");

            modelBuilder.ApplyConfiguration(new Configurations.CourseConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CourseAssignmentConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.EnrollmentConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.InstructorConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.OfficeAssignmentConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.StudentConfiguration());
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
