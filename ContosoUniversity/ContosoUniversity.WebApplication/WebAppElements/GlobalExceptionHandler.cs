using ContosoUniversity.WebApplication.Models;

using Microsoft.AspNetCore.Diagnostics;

using System.Net;

namespace ContosoUniversity.WebApplication.WebAppElements
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            string exceptionMessage = exception.Message;
            _logger.LogError(exception, $"An error has occured : {exceptionMessage}");

            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(new ErrorMessageModel()
            {
                Title = "Exception occured",
                Details = exceptionMessage,
                Type = exception.GetType().Name
            }, cancellationToken);

            return true;
        }
    }
}
