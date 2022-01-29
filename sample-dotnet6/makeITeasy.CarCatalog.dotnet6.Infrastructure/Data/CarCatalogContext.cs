
using makeITeasy.CarCatalog.dotnet6.Infrastructure.Data.Configurations;
using makeITeasy.CarCatalog.dotnet6.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
namespace makeITeasy.CarCatalog.dotnet6.Infrastructure.Data
{
    public partial class CarCatalogContext : DbContext
    {
public virtual DbSet<Brand> Brands { get; set; }
public virtual DbSet<Car> Cars { get; set; }
public virtual DbSet<CarDetail> CarDetails { get; set; }
public virtual DbSet<Country> Countries { get; set; }

public CarCatalogContext(DbContextOptions<CarCatalogContext> options) : base(options)
{
 }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Scaffolding:ConnectionString", "Data Source=(local);Initial Catalog=makeITeasy.CarCatalog.Database;Integrated Security=true");

            modelBuilder.ApplyConfiguration(new Configurations.BrandConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CarConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CarDetailConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CountryConfiguration());
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
