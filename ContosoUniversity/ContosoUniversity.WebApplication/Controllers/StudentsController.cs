using AutoMapper;

using ContosoUniversity.Core.Queries.StudentQueries;
using ContosoUniversity.Models;
using ContosoUniversity.WebApplication.Models.StudentModels;

using Dawn;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace ContosoUniversity.WebApplication.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public StudentsController(IMediator mediator, IMapper mapper)
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
                var students =
                    await _mediator.Send(new GenericQueryWithProjectCommand<Student, StudentEditViewModel>(new BasicinstructorQuery() { ID = id }));

                StudentEditViewModel? model = students.Results.FirstOrDefault();

                return base.PartialView(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] StudentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                CommandResult<Student> result = await _mediator.Send(new UpdateEntityCommand<Student>(_mapper.Map<Student>(model)));

                if (result.Result == CommandState.Success)
                {
                    return Ok(_mapper.Map<StudentEditViewModel>(result.Entity));
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "An error has occured");
        }

        public async Task<IActionResult> Details(int id)
        {
            QueryResult<StudentDetailsViewModel> result = 
                await _mediator.Send(new GenericQueryWithProjectCommand<Student, StudentDetailsViewModel>(new BasicinstructorQuery() { ID = id }));

            return PartialView(result.Results.FirstOrDefault());
        }
    }
}