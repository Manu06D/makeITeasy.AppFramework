using makeITeasy.AppFramework.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using System.Threading.Channels;

namespace ContosoUniversity.Infrastructure.Persistence
{
    public class DatabaseInterceptor : ISaveChangesInterceptor
    {
        private readonly ChannelWriter<IBaseEntity> channelWriter;

        public DatabaseInterceptor(ChannelWriter<IBaseEntity> channelWriter)
        {
            this.channelWriter = channelWriter;
        }

        public void SaveChangesFailed(DbContextErrorEventData eventData)
        {
        }

        public Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            return result;
        }

        public ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            return new ValueTask<int>(result);
        }

        public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            return result;
        }

        public async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            context.ChangeTracker.DetectChanges();
            
            foreach (var entry in context.ChangeTracker.Entries().Select(x => x.Entity).Where(x => x != null && x is IBaseEntity)) 
            {
                await channelWriter.WriteAsync(entry as IBaseEntity);
            }
            
            return result;
        }
    }
}
