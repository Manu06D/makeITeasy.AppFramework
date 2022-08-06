using AutoMapper;

using ContosoUniversity.Core.Queries.CourseQueries;
using ContosoUniversity.Models;
using ContosoUniversity.WebApplication.Models.CourseModels;

using Dawn;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Queries;
using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace ContosoUniversity.WebApplication.Controllers
{
    public class CoursesController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public CoursesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            Guard.Argument(id, nameof(id)).Positive();

            if (id > 0)
            {
                CourseEditViewModel? model =
                    await _mediator.Send(new GenericFindUniqueWithProjectCommand<Course, CourseEditViewModel>(new BasicCourseQuery() { ID = id }));

                return base.PartialView(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] CourseEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                CommandResult<Course> result = await _mediator.Send(new UpdateEntityCommand<Course>(_mapper.Map<Course>(model)));

                if (result.Result == CommandState.Success)
                {
                    return Ok(_mapper.Map<CourseEditViewModel>(result.Entity));
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "An error has occured");
        }

        public async Task<IActionResult> Details(int id)
        {
            CourseDetailsViewModel result = 
                await _mediator.Send(new GenericFindUniqueWithProjectCommand<Course, CourseDetailsViewModel>(new BasicCourseQuery() { ID = id }));

            return PartialView(result);
        }
    }
}
