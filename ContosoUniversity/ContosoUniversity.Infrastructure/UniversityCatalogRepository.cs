using AutoMapper;

using ContosoUniversity.Infrastructure.Data;

using makeITeasy.AppFramework.Infrastructure.Persistence;
using makeITeasy.AppFramework.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoUniversity.Infrastructure
{
    public class UniversityCatalogRepository<T> : BaseEfRepository<T, ContosoUniversityDbContext> where T : class, IBaseEntity
    {
        public UniversityCatalogRepository(ContosoUniversityDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}
