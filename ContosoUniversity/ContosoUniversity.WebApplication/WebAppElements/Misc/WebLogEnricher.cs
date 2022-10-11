using Serilog.Core;
using Serilog.Events;

namespace ContosoUniversity.WebApplication.WebAppElements.Misc
{
    public class WebLogEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private const string logPath = "Path";

        public WebLogEnricher(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public WebLogEnricher() : this(new HttpContextAccessor())
        {
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            string? value = _contextAccessor.HttpContext?.Request?.Path.Value;

            if (!string.IsNullOrEmpty(value))
            {
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(logPath, value));
            }
        }
    }
}
