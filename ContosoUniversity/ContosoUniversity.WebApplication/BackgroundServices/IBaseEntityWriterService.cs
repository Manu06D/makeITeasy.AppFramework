using makeITeasy.AppFramework.Models;

using System.Threading.Channels;

namespace ContosoUniversity.WebApplication.BackgroundServices
{
    public class IBaseEntityWriterService : BackgroundService
    {
        private readonly ChannelWriter<IBaseEntity> _channelWriter;
        private readonly ILogger<IBaseEntityWriterService> _logger;

        public IBaseEntityWriterService(ChannelWriter<IBaseEntity> channelWriter, ILogger<IBaseEntityWriterService> logger)
        {
            _channelWriter = channelWriter;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //await _channelWriter.WriteAsync()
            }
        }
    }
}
