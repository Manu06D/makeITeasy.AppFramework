using AutoMapper;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.dotnet6.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet6.Core.Services.Queries.BrandQueries;
using makeITeasy.CarCatalog.dotnet6.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet6.Models;
using makeITeasy.CarCatalog.dotnet6.WebApp.Models;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.Diagnostics;

namespace makeITeasy.CarCatalog.dotnet6.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IBrandService _brandService;

        public HomeController(ILogger<HomeController> logger, IMediator mediator, IMapper mapper, IBrandService brandService)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
            _brandService = brandService;
        }

        public async Task<IActionResult> Index()
        {

            IList<Brand> brands = (await _brandService.ListAllAsync());
            Models.Views.Home.IndexViewModel model = new()
            {
                AllBrandNames = brands.Select(x => new SelectListItem(x.Name, x.Name)).ToList(),
                AllBrandIds = brands.Select(x => new SelectListItem(x.Id.ToString(), x.Id.ToString())).ToList()
            };

            return base.View(model);
        }

        public IActionResult IndexWithBlazor()
        {
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id > 0)
            {
                CarEditViewModel model =
                    await _mediator.Send(new GenericFindUniqueWithProjectCommand<Car, CarEditViewModel>(
                            new BaseCarQuery() { ID = id, Includes = new List<System.Linq.Expressions.Expression<Func<Car, object>>>() { x => x.Brand } }));

                QueryResult<Brand> output = await _mediator.Send(new GenericQueryCommand<Brand>(new BaseBrandQuery()));

                model.Brands = output.Results.ToList();

                return base.PartialView(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] CarEditViewModel model)
        {
            //if (ModelState.IsValid)
            //{
                CommandResult<Car> result = await _mediator.Send(new UpdateEntityCommand<Car>(_mapper.Map<Car>(model)));

                if (result.Result == CommandState.Success)
                {
                    return Ok(model);
                }
            //}

            return StatusCode(StatusCodes.Status500InternalServerError, "An error has occured");
        }

        public async Task<IActionResult> CarDetails(int id)
        {
            Car result = await _mediator.Send(new GenericFindUniqueCommand<Car>(new BaseCarQuery() { ID = id, IncludeStrings = new List<string>() { "Brand" } }));

            return PartialView(result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}