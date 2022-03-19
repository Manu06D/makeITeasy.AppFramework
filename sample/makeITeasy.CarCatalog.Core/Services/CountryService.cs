using makeITeasy.AppFramework.Core.Services;
using makeITeasy.CarCatalog.Core.Services.Interfaces;
using makeITeasy.CarCatalog.Models;

namespace makeITeasy.CarCatalog.Core.Services
{
    public class CountryService : BaseEntityService<Country>, ICountryService
    {
    }
}
