using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet9.Models;

using MediatR;

using Microsoft.AspNetCore.Components;

namespace makeITeasy.CarCatalog.dotnet9.WebApp.Components.Pages
{
    public partial class Home
    {
        [Inject]
        public required IMediator Mediator { get; set; }

        public IQueryable<Car>? cars;

        protected override async Task OnInitializedAsync()
        {
            var dbQuery = await Mediator.Send(new GenericQueryCommand<Car>(new BaseCarQuery() { }));
            if(dbQuery.Results != null)
            {
                cars = dbQuery.Results.AsQueryable();
            }
        }
    }
}