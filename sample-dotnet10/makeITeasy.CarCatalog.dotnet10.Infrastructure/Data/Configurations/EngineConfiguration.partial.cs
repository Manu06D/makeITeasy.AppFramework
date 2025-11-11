using makeITeasy.CarCatalog.dotnet10.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace makeITeasy.CarCatalog.dotnet10.Infrastructure.Data.Configurations
{
    public partial class EngineConfiguration
    {
        partial void OnConfigurePartial(EntityTypeBuilder<Engine> entity)
        {
            entity.OwnsOne(x => x.Details, cb =>
            {
                cb.ToJson();
                //cb.Property(x => x.HasTurbo);
                //cb.Property(x => x.PowerHorse);
                cb.OwnsMany(x => x.Characteristics);
            });
        }
    }
}
