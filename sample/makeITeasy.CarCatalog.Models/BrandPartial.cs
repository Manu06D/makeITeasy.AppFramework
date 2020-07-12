using System;
using System.Collections.Generic;
using System.Text;

namespace makeITeasy.CarCatalog.Models
{
    public partial class Brand
    {
        public override object DatabaseID { get => Id; set => throw new NotImplementedException(); }

    }
}
