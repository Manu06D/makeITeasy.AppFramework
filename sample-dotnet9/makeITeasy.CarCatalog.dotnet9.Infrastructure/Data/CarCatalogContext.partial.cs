using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace makeITeasy.CarCatalog.dotnet9.Infrastructure.Data
{
    public partial class CarCatalogContext
    {
        //store DateTime as UTC in the database and read it as local
        public class DateTimeUtcConverter : ValueConverter<DateTime, DateTime>
        {
            public DateTimeUtcConverter() : base(
                                                    d => d.ToUniversalTime(),
                                                    d =>
                                                    DateTime.SpecifyKind(d, DateTimeKind.Utc).ToLocalTime())
                                                    //or
                                                    //TimeZoneInfo frenchTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                                                    //DateTime frenchDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, frenchTimeZone);
            { }
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            //ApplyDateTimeConverterToEveryDateTimeProperty(modelBuilder);
        }

        private static void ApplyDateTimeConverterToEveryDateTimeProperty(ModelBuilder modelBuilder)
        {
            var dateTimeUtcConverter = new DateTimeUtcConverter();

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeUtcConverter);
                    }
                }
            }
        }
    }
}
