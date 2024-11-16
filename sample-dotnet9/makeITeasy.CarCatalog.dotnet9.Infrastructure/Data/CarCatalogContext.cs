
using makeITeasy.CarCatalog.dotnet9.Infrastructure.Data.Configurations;
using makeITeasy.CarCatalog.dotnet9.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
namespace makeITeasy.CarCatalog.dotnet9.Infrastructure.Data
{
    public partial class CarCatalogContext : DbContext
    {
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Car> Cars { get; set; }
        public virtual DbSet<CarDetail> CarDetails { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Engine> Engines { get; set; }

        public CarCatalogContext()
        {
        }

        public CarCatalogContext(DbContextOptions<CarCatalogContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        modelBuilder.ApplyConfiguration(new Configurations.BrandConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CarConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CarDetailConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CountryConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.EngineConfiguration());

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
