using System;
using System.Collections.Generic;
using System.Text.Json;
using makeITeasy.AppFramework.Web.DataTables.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace makeITeasy.AppFramework.Web.Helpers
{
    public static class DatatableHelpers
    {
        public static void RegisterDatatablesService(this IServiceCollection services)
        {
            services.RegisterDataTables(ctx =>
            {
                string appJson = ctx.ValueProvider?.GetValue("appJson").FirstValue ?? "{}";
                return JsonSerializer.Deserialize<IDictionary<string, object>>(appJson);
            }, true);
        }
    }
}
