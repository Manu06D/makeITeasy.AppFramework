using makeITeasy.AppFramework.Core.Services;
using makeITeasy.CarCatalog.dotnet7.Models;
using makeITeasy.CarCatalog.dotnet7.Core.Services.Interfaces;
using FluentValidation;
using makeITeasy.AppFramework.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace makeITeasy.CarCatalog.dotnet7.Core.Services
{
    public class BrandService : BaseEntityService<Brand>, IBrandService
    {
        public BrandService(IAsyncRepository<Brand> entityRepository, ILogger<BaseEntityService<Brand>> logger, IValidator<Brand>? validator = null) 
            : base(entityRepository, logger, validator)
        {
            
        }
    }
}
