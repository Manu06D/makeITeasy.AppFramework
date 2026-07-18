using AutoMapper;

using makeITeasy.AppFramework.Infrastructure.EntityFramework.Persistence;
using makeITeasy.CarCatalog.dotnet10.Core.Ports;
using makeITeasy.CarCatalog.dotnet10.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet10.Models;
using makeITeasy.CarCatalog.dotnet10.Models.Custom;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;


namespace makeITeasy.CarCatalog.dotnet10.Infrastructure.Repositories
{
    public class CarRepository : BaseEfRepository<Car, CarCatalogContext>, ICarRepository
    {
        public CarRepository(IDbContextFactory<CarCatalogContext> dbFactory, IMapper mapper) : base(dbFactory, mapper)
        {
        }

        public async Task<List<BrandGroupByCarCount>> GroupByBrandAndCountAsync()
        {
            var query = GetDbContext().Cars.GroupBy(x => x.Brand.Name).Select(x => new BrandGroupByCarCount() { BrandName = x.Key, CarCount = x.Count() });

            return await query.ToListAsync();
        }

        public async Task<int> AddCarsInResilientTransactionAsync(IEnumerable<Car> cars, CancellationToken cancellationToken = default)
        {
            IExecutionStrategy strategy = GetDbContext().Database.CreateExecutionStrategy();

            int totalSaved = 0;

            await strategy.ExecuteAsync(async () =>
            {
                totalSaved = 0;

                CarCatalogContext context = GetDbContext();
                await using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
                {
                    foreach (Car car in cars)
                    {
                        context.Cars.Add(car);
                        totalSaved += await context.SaveChangesAsync(cancellationToken);
                    }

                    await transaction.CommitAsync(cancellationToken);
                }
            });

            return totalSaved;
        }
    }
}
