using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet9.Models;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace makeITeasy.CarCatalog.dotnet9.WebApp.Components.Apis
{
    [Route("api/Car")]
    [ApiController]
    public class CarApiController(IMediator mediator)
    {
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CarResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<CarResponse> GetCarById(int id)
        {
            return await mediator.Send(new GenericFindUniqueWithProjectCommand<Car, CarResponse>(new BaseCarQuery() { ID = id }));
        }

        public class CarResponse : IMapFrom<Car>
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}
