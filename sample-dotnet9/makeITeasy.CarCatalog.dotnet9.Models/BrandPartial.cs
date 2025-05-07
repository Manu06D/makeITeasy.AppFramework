using FluentValidation;

using makeITeasy.AppFramework.Models;

namespace makeITeasy.CarCatalog.dotnet9.Models
{
    public partial class Brand : ITimeTrackingEntity
    {
        public object DatabaseID { get => Id; }
    }

    public class BrandValidator : AbstractValidator<Brand>
    {
        public BrandValidator()
        {
            RuleFor(x => x.CountryId).GreaterThan(0).When(x => x.Country is null).WithMessage("Brand should be linked to a country");
            RuleFor(x => x.CountryId).Equal(0).When(x => x.Country?.Id == 0).WithMessage("Brand should be linked to a country");
        }
    }
}
