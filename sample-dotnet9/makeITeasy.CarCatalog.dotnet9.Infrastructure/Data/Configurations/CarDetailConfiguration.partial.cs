using makeITeasy.CarCatalog.dotnet9.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace makeITeasy.CarCatalog.dotnet9.Infrastructure.Data.Configurations
{
    public partial class CarDetailConfiguration
    {
        partial void OnConfigurePartial(EntityTypeBuilder<CarDetail> entity)
        {
            entity.OwnsOne(post => post.CarDetails, builder => { builder.ToJson(); });
        }
    }
}
