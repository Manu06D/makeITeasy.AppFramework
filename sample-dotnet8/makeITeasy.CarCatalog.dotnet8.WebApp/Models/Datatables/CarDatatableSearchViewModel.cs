using makeITeasy.CarCatalog.dotnet8.Core.Services.Queries.CarQueries;

namespace makeITeasy.CarCatalog.dotnet8.WebApp.Models.Datatables
{
    public class CarDatatableSearchViewModel : BaseCarQuery
    {
        //Has to set this variable cause checkbox can't bind to nullable bool (tri-state)
        public bool IsModernCarSearch { get { return IsModernCar.GetValueOrDefault(); } set { IsModernCar = value ? true : null; } }
    }
}
