using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using makeITeasy.AppFramework.Core.Infrastructure.Persistence;
using makeITeasy.CarCatalog.Core.Ports;
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.Models.Custom;

namespace makeITeasy.CarCatalog.Infrastructure.Repositories
{
    public class CarRepository : BaseEfRepository<Car, CarCatalogContext>, ICarRepository
    {
        public CarRepository(CarCatalogContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public List<BrandGroupByCarCount> OwnRepositoryMethod()
        {
            var query = _dbContext.Cars.GroupBy(x => x.Brand.Name).Select(x => new BrandGroupByCarCount() { BrandName = x.Key, CarCount = x.Count() });

            return query.ToList();
        }
    }
}
