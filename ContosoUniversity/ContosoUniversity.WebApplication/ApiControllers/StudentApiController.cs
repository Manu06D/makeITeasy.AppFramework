using ContosoUniversity.Models;
using ContosoUniversity.WebApplication.Models.Datatables;

using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.AppFramework.Models;
using makeITeasy.AppFramework.Web.DataTables.AspNetCore;
using makeITeasy.AppFramework.Web.Filters;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace ContosoUniversity.WebApplication.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentApiController
    {
        private readonly IMediator _mediator;

        public StudentApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[ServiceFilter(typeof(DatatableExceptionFilter))]
        [HttpPost("/api/student/search", Name = nameof(StudentDatatableSearchRequest))]
        public async Task<IActionResult> StudentDatatableSearchRequest(IDataTablesRequest request)
        {
            var searchQuery = request?.GetSearchInformation<StudentDatatable, Student, ITransactionSpecification<Student>>();

            searchQuery.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;

            QueryResult<StudentDatatableViewModel> output = await _mediator.Send(new GenericQueryWithProjectCommand<Student, StudentDatatableViewModel>(searchQuery, true));

            var response = DataTablesResponse.Create(request, output.TotalItems, output.TotalItems, output.Results);

            return new DataTablesJsonResult(response, true);
        }
    }
}
