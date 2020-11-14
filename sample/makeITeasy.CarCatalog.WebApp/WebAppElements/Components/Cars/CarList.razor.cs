using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.WebApp.Models.Datatables;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace makeITeasy.CarCatalog.WebApp.WebAppElements.Components.Cars
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