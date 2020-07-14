﻿using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.Models;

namespace makeITeasy.CarCatalog.Core.Domains.BrandDomain.Queries
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
