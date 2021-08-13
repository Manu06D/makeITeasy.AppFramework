using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using Serilog;

using System.Threading;
using System.Threading.Tasks;

namespace makeITeasy.CarCatalog.Infrastructure.Persistence
{
    public class DatabaseInterceptor : ISaveChangesInterceptor
    {
        private readonly Serilog.ILogger _logger = Log.ForContext(typeof(DatabaseInterceptor));
        public void SaveChangesFailed(DbContextErrorEventData eventData)
        {
        }

        public Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            _logger.Information("SaveChangesFailedAsync");
            
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
            var context = eventData.Context;
            context.ChangeTracker.DetectChanges();
            foreach(var entry in context.ChangeTracker.Entries())
            {
                var message = entry.State switch
                {
                    EntityState.Deleted => $"{entry.Entity.GetType().Name} deleted",
                    EntityState.Modified => $"{entry.Entity.GetType().Name} modified",
                    EntityState.Added => $"{entry.Entity.GetType().Name} added",
                    _ => null
                };

                if(message != null)
                {
                    _logger.Information(message);
                }
            }

            return result;
        }

        public ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            return new ValueTask<InterceptionResult<int>>(result);
        }
    }
}