﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.AppFramework.Web.DataTables.AspNetCore;
using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.WebApp.Models.Datatables;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace makeITeasy.CarCatalog.WebApp.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarApiController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CarApiController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpPost("/api/car/search", Name = nameof(CarDatatableSearchRequest))]
        public async Task<IActionResult> CarDatatableSearchRequest(IDataTablesRequest request)
        {
            var searchQuery = request?.GetSearchInformation<CarDatatable, Car>();

            QueryResult<CarDatatableViewModel> output = await _mediator.Send(new GenericQueryWithProjectCommand<Car, CarDatatableViewModel>(searchQuery));

            var response = DataTablesResponse.Create(request, output.TotalItems, output.TotalItems, output.Results);

            return new DataTablesJsonResult(response, true);
        }
    }
}
