using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.dotnet6.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet6.WebApp.Models.Datatables;
using makeITeasy.CarCatalog.dotnet6.Models;

using MediatR;

using Microsoft.AspNetCore.Components;

namespace makeITeasy.CarCatalog.dotnet6.WebApp.WebAppElements.Components.Cars
{
    public partial class CarList
    {
        public string SearchValue { get; set; } = string.Empty;

        QueryResult<CarDatatableViewModel> requests = new QueryResult<CarDatatableViewModel>();

        [Inject]
        private IMediator _mediator { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await LoadRequests();
        }

        async Task LoadRequests()
        {
            var query = new BaseCarQuery();

            if (!string.IsNullOrEmpty(SearchValue))
            {
                query.Name = SearchValue;
            }

            requests = await _mediator.Send(new GenericQueryWithProjectCommand<Car, CarDatatableViewModel>(query));
        }
    }
}