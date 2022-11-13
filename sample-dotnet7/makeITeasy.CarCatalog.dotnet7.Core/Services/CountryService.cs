using makeITeasy.AppFramework.Core.Services;
using makeITeasy.CarCatalog.dotnet7.Models;
using makeITeasy.CarCatalog.dotnet7.Core.Services.Interfaces;

namespace makeITeasy.CarCatalog.dotnet7.Core.Services
{
    public class CountryService : BaseEntityService<Country>, ICountryService
    {
    }
}
