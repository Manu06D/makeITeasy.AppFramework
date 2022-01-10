﻿using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet6.Models;

namespace makeITeasy.CarCatalog.dotnet6.Core.Services.Queries.BrandQueries
{
    public class BaseBrandQuery : BaseQuery<Brand>
    {
        public int ID { get; set; }

        public override void BuildQuery()
        {
            if (ID > 0)
            {
                AddFunctionToCriteria(x => x.Id == ID);
            }
        }
    }
}
