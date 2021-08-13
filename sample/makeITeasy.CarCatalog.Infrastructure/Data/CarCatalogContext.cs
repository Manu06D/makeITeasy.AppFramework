
using makeITeasy.CarCatalog.Infrastructure.Data.Configurations;
using makeITeasy.CarCatalog.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System;
namespace makeITeasy.CarCatalog.Infrastructure.Data
{
    public partial class CarCatalogContext : DbContext
    {
public virtual DbSet<Brand> Brands { get; set; }
public virtual DbSet<Car> Cars { get; set; }
public virtual DbSet<Country> Countries { get; set; }

public CarCatalogContext(DbContextOptions<CarCatalogContext> options) : base(options)
{
 }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Scaffolding:ConnectionString", "Data Source=(local);Initial Catalog=makeITeasy.CarCatalog.Database;Integrated Security=true");

            modelBuilder.ApplyConfiguration(new Configurations.BrandConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CarConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CountryConfiguration());
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
