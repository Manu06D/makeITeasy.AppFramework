using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using makeITeasy.AppFramework.Core.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace makeITeasy.CarCatalog.Tests
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            bool IsFrameworkCommandResult = request.GetType().GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICommandEntity<>));
            if (IsFrameworkCommandResult)
            {
                var entity = request.GetType().GetInterfaces()[0].GetProperty("Entity")?.GetValue(request, null);
                _logger.LogInformation($"Processing {typeof(TRequest).Name} with Entity {TestHelper.Dump(entity)}");
            }

            var response = await next();

            if (typeof(TResponse).IsGenericType && typeof(TResponse).BaseType == typeof(CommandResult))
            {
                var entityResponse = typeof(TResponse).GetProperty("Entity")?.GetValue(response, null);
                _logger.LogInformation($"Processed {TestHelper.Dump(entityResponse)}");
            }

            return response;
        }
    }
}
