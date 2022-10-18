using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using makeITeasy.AppFramework.Infrastructure.Persistence;
using makeITeasy.CarCatalog.dotnet6.Core.Ports;
using makeITeasy.CarCatalog.dotnet6.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet6.Models.Custom;
using makeITeasy.CarCatalog.dotnet6.Models;

using Microsoft.EntityFrameworkCore;

namespace makeITeasy.CarCatalog.dotnet6.Infrastructure.Repositories
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
    }
}
