using makeITeasy.AppFramework.Core.Services;
using makeITeasy.CarCatalog.dotnet10.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet10.Models;

namespace makeITeasy.CarCatalog.dotnet10.Core.Services
{
    public class CountryService : BaseEntityService<Country>, ICountryService
    {
    }
}
