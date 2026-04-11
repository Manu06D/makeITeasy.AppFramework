using makeITeasy.AppFramework.Core.Commands;

using MediatR;

using Microsoft.Extensions.Logging;

namespace makeITeasy.CarCatalog.dotnet10.Tests.TestsSetup
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        private readonly MediatRLog _mediatRLog;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, MediatRLog mediatRLog)
        {
            _logger = logger;
            _mediatRLog = mediatRLog;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            bool IsFrameworkCommandResult = request.GetType().GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICommandEntity<>));
            if (IsFrameworkCommandResult)
            {
                var entity = request.GetType().GetInterfaces()[0].GetProperty("Entity")?.GetValue(request, null);
                string message = $"Processing {typeof(TRequest).Name} with Entity {TestHelper.Dump(entity)}";
                _logger.LogInformation(message);
                _mediatRLog.Counter++;
                _mediatRLog.Logs.Add(message);
            }

            var response = await next();

            if (typeof(TResponse).IsGenericType && typeof(TResponse).BaseType == typeof(CommandResult))
            {
                var entityResponse = typeof(TResponse).GetProperty("Entity")?.GetValue(response, null);
                string message = $"Processed {TestHelper.Dump(entityResponse)}";
                _logger.LogInformation(message);
                _mediatRLog.Counter++;
                _mediatRLog.Logs.Add(message);
            }
            else if (typeof(TResponse) == typeof(CommandResult))
            {
                var entityResponse = typeof(TResponse).GetProperty("Result")?.GetValue(response, null);
                string message = $"Processed with result = {TestHelper.Dump(entityResponse)}";
                _logger.LogInformation(message);
                _mediatRLog.Counter++;
                _mediatRLog.Logs.Add(message);
            }

            return response;
        }
    }
}
