using FluentValidation;

using makeITeasy.CarCatalog.dotnet9.Models.DynamicModels;

namespace makeITeasy.CarCatalog.dotnet9.Models
{
    public partial class CarDetail
    {
        public object DatabaseID { get => Id; }
        public CarDetailsModel CarDetails { get; set; }
    }

    public class CarValidator : AbstractValidator<Car>
    {
        public CarValidator()
        {
            RuleFor(x => x.Name).MinimumLength(2).MaximumLength(50).WithMessage("Name of Car length should be between 2 and 10");
        }
    }
}
