
using makeITeasy.CarCatalog.dotnet8.Infrastructure.Data.Configurations;
using makeITeasy.CarCatalog.dotnet8.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
namespace makeITeasy.CarCatalog.dotnet8.Infrastructure.Data
{
    public partial class CarCatalogContext : DbContext
    {
public virtual DbSet<Brand> Brands { get; set; }
public virtual DbSet<Car> Cars { get; set; }
public virtual DbSet<CarDetail> CarDetails { get; set; }
public virtual DbSet<CompositeKeySubTable> CompositeKeySubTables { get; set; }
public virtual DbSet<CompositeKeyTable> CompositeKeyTables { get; set; }
public virtual DbSet<Country> Countries { get; set; }
public virtual DbSet<Engine> Engines { get; set; }

public CarCatalogContext(DbContextOptions<CarCatalogContext> options) : base(options)
{
 }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        modelBuilder.ApplyConfiguration(new Configurations.BrandConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CarConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CarDetailConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CompositeKeySubTableConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CompositeKeyTableConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CountryConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.EngineConfiguration());

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
