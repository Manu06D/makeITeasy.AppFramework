using FluentValidation;

using makeITeasy.CarCatalog.dotnet10.Models.DynamicModels;

using System.ComponentModel.DataAnnotations.Schema;

namespace makeITeasy.CarCatalog.dotnet10.Models
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
