using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.Core.Domains.BrandDomain.Queries;
using makeITeasy.CarCatalog.Core.Domains.CarDomain;
using makeITeasy.CarCatalog.Core.Domains.CarDomain.Queries;
using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.WebApp.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace makeITeasy.CarCatalog.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id > 0)
            {
                var cars =
                    await _mediator.Send(new GenericQueryWithProjectCommand<Car, CarEditViewModel>(
                            new BaseCarQuery() { ID = id, Includes = new List<System.Linq.Expressions.Expression<Func<Car, object>>>() { x => x.Brand } }));

                CarEditViewModel model = cars.Results.FirstOrDefault();

                QueryResult<Brand> output = await _mediator.Send(new GenericQueryCommand<Brand>(new BaseBrandQuery()));

                model.Brands = output.Results.ToList();

                return base.PartialView(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] CarEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                CommandResult<Car> result = await _mediator.Send(new UpdateEntityCommand<Car>(_mapper.Map<Car>(model)));

                if (result.Result == CommandState.Success)
                {
                    return Ok(model);
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "An error has occured");
        }

        public async Task<IActionResult> CarDetails(int id)
        {
            QueryResult<Car> result = await _mediator.Send(new GenericQueryCommand<Car>(new BaseCarQuery() { ID = id, IncludeStrings = new List<string>() { "Brand" } }));

            return PartialView(result.Results.FirstOrDefault());
        }
    }
}
