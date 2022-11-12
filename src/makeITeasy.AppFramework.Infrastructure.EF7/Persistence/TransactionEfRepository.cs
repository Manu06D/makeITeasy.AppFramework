using AutoMapper;
using makeITeasy.AppFramework.Models;
using makeITeasy.AppFramework.Core.Queries;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace makeITeasy.AppFramework.Infrastructure.EF7.Persistence
{
    public class TransactionEfRepository<T, U> : BaseEfRepository<T, U> where T : class, IBaseEntity where U : DbContext
    {
        public TransactionEfRepository(IDbContextFactory<U> dbFactory, IMapper mapper) : base(dbFactory, mapper)
        {
        }

        public override async Task<int> ApplyCountIfNeededAsync(IQueryable<T>? filteredSet, bool includeCount)
        {
            if (includeCount && filteredSet != null)
            {
                //Can't async/await otherwise the error "A TransactionScope must be disposed on the same thread that it was created"  will pop
                return filteredSet.CountAsync().Result;
            }

            return await Task.FromResult(0);
        }

        public override async Task<QueryResult<T>> ListAsync(ISpecification<T> spec, bool includeCount = false)
        {
            async Task<QueryResult<T>> functionToExecute() => await base.ListAsync(spec, includeCount);

            IsolationLevel? isolationLevel = GetIsolationLevelFromSpec(spec);

            if (isolationLevel.HasValue)
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = isolationLevel.Value }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = await functionToExecute();
                    scope.Complete();
                    return result;
                }
            }
            else
            {
                return await functionToExecute();
            }
        }

        public override async Task<QueryResult<X>> ListWithProjectionAsync<X>(ISpecification<T> spec, bool includeCount = false)
        {
            async Task<QueryResult<X>> functionToExecute() => await base.ListWithProjectionAsync<X>(spec, includeCount);

            IsolationLevel? isolationLevel = GetIsolationLevelFromSpec(spec);

            if (isolationLevel.HasValue)
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = isolationLevel.Value }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = await functionToExecute();
                    scope.Complete();
                    return result;
                }
            }
            else
            {
                return await functionToExecute();
            }
        }

        private static IsolationLevel? GetIsolationLevelFromSpec(ISpecification<T> spec)
        {
            if (spec.GetType().GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITransactionSpecification<>)))
            {
                return ((ITransactionSpecification<T>)spec).IsolationLevel;
            }

            return null;
        }
    }
}
