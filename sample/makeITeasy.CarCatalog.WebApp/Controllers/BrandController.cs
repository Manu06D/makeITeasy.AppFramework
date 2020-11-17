using System.Threading.Tasks;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.Core.Services.Queries.BrandQueries;
using makeITeasy.CarCatalog.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace makeITeasy.CarCatalog.WebApp.Controllers
{
    public class BrandController : Controller
    {
        private readonly IMediator _mediator;

        public BrandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            QueryResult<Brand> output = await _mediator.Send(new GenericQueryCommand<Brand>(new BaseBrandQuery()));
            return View(output);
        }
    }
}
