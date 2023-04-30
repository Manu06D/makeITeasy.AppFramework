using makeITeasy.AppFramework.Models;

using Newtonsoft.Json;

using System.Threading.Channels;

namespace ContosoUniversity.WebApplication.BackgroundServices
{
    public class IBaseEntityReaderService : BackgroundService
    {
        private readonly ChannelReader<IBaseEntity> _channelReader;
        private readonly ILogger<IBaseEntityReaderService> _logger;
        public IBaseEntityReaderService(ChannelReader<IBaseEntity> channelReader, ILogger<IBaseEntityReaderService> logger)
        {
            _channelReader = channelReader;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(await _channelReader.WaitToReadAsync(stoppingToken))
            {
                try
                {
                    IBaseEntity entity = await _channelReader.ReadAsync(stoppingToken);
                    _logger.LogInformation($"An entity has been updated : {JsonConvert.SerializeObject(entity)}");
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "An error has occured whiled reading channel");
                    throw;
                }
            }
        }
    }
}
