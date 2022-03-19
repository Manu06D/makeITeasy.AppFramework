using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace ContosoUniversity.WebApplication.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public InstructorsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}