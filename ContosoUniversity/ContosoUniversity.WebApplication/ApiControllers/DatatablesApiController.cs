using ContosoUniversity.Models;
using ContosoUniversity.WebApplication.Models.Datatables;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.AppFramework.Models;
using makeITeasy.AppFramework.Web.DataTables.AspNetCore;
using makeITeasy.AppFramework.Web.Models;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace ContosoUniversity.WebApplication.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatatablesApiController
    {
        private readonly IMediator _mediator;

        public DatatablesApiController  (IMediator mediator)
        {
            _mediator = mediator;
        }


        private async Task<IActionResult> buildGenericDatatableQuery<TDatatable, TDatatableViewModel, TEntity>(IDataTablesRequest request) 
            where TDatatable : IDatatableBaseConfiguration where TEntity : class, IBaseEntity where TDatatableViewModel : class
        {
            var searchQuery = request?.GetSearchInformation<TDatatable, TEntity, ITransactionSpecification<TEntity>>();

            searchQuery.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;

            QueryResult<TDatatableViewModel> output = await _mediator.Send(new GenericQueryWithProjectCommand<TEntity, TDatatableViewModel>(searchQuery, true));

            var response = DataTablesResponse.Create(request, output.TotalItems, output.TotalItems, output.Results);

            return new DataTablesJsonResult(response, true);
        }

        [HttpPost("/api/datatables/students/search", Name = nameof(StudentDatatableSearchRequest))]
        public async Task<IActionResult> StudentDatatableSearchRequest(IDataTablesRequest request)
        {
            return await buildGenericDatatableQuery<StudentDatatable, StudentDatatableViewModel, Student> (request);
        }

        [HttpPost("/api/datatables/courses/search", Name = nameof(CoursesDatatableSearchRequest))]
        public async Task<IActionResult> CoursesDatatableSearchRequest(IDataTablesRequest request)
        {
            return await buildGenericDatatableQuery<CourseDatatable, CourseDatatableViewModel, Course>(request);
        }


        [HttpPost("/api/datatables/studentEnrollments/search", Name = nameof(StudentEnrollmentsDatatableSearchRequest))]
        public async Task<IActionResult> StudentEnrollmentsDatatableSearchRequest(IDataTablesRequest request)
        {
            return await buildGenericDatatableQuery<StudentEnrollmentDatatable, StudentEnrollmentDatatableViewModel, Enrollment>(request);
        }

        [HttpPost("/api/datatables/instructors/search", Name = nameof(InstructorsDatatableSearchRequest))]
        public async Task<IActionResult> InstructorsDatatableSearchRequest(IDataTablesRequest request)
        {
            return await buildGenericDatatableQuery<InstructorDatatable, InstructorDatatableViewModel, Instructor>(request);
        }
    }
}
