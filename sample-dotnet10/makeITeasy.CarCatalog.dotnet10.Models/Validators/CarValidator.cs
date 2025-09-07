using FluentValidation;

namespace makeITeasy.CarCatalog.dotnet10.Models.Validators
{
    public class CarValidator : AbstractValidator<Car>
    {
        public CarValidator()
        {
            RuleFor(x => x.Name).MinimumLength(2).MaximumLength(50).WithMessage("Name of Car length should be between 2 and 10");
        }
    }
}
