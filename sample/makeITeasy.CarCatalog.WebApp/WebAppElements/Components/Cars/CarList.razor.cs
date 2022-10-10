using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.WebApp.Models.Datatables;
using makeITeasy.CarCatalog.Models;

using MediatR;

using Microsoft.AspNetCore.Components;

namespace makeITeasy.CarCatalog.WebApp.WebAppElements.Components.Cars
{
    public partial class CarList
    {
        public string SearchValue { get; set; } = string.Empty;

        QueryResult<CarDatatableViewModel> requests = new();

        [Inject]
        private IMediator? _mediator { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await LoadRequests();
        }

        private async Task LoadRequests()
        {
            var query = new BaseCarQuery();

            if (!string.IsNullOrEmpty(SearchValue))
            {
                query.Name = SearchValue;
            }

            if(_mediator != null)
            {
                requests = await _mediator.Send(new GenericQueryWithProjectCommand<Car, CarDatatableViewModel>(query));
            }
        }
    }
}