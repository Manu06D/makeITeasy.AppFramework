using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.dotnet7.WebApp.Models.Datatables;
using makeITeasy.CarCatalog.dotnet7.Models;

using MediatR;

using Microsoft.AspNetCore.Components;
using makeITeasy.CarCatalog.dotnet7.Core.Services.Queries.CarQueries;

namespace makeITeasy.CarCatalog.dotnet7.WebApp.WebAppElements.Components.Cars
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