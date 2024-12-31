using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

using Radzen;
using Radzen.Blazor;

namespace makeITeasy.CarCatalog.dotnet9.WebApp.Components.Layout
{
    public partial class Grid <TEntity, TEntityQuery>  : ComponentBase
        where TEntityQuery : ISpecification<TEntity>, new()
        where TEntity : class, IBaseEntity, new()
    {
        public required RadzenDataGrid<TEntity> grid;
        private IEnumerable<TEntity>? gridEntities = [];
        private bool isLoading;
        private int totalCount;

        [Inject]
        private ILogger<TEntity> Logger { get; set; } = default!;
        [Inject]
        private DialogService DialogService { get; set; } = default!;
        [Inject]
        private NotificationService NotificationService { get; set; } = default!;

        [Inject]
        public required IBaseEntityService<TEntity> EntityService { get; set; }
        [Parameter]
        public int PageSize { get; set; } = 20;
        [Parameter]
        public bool LoadOnDisplay { get; set; }
        [Parameter]
        public bool ShowReloadButton { get; set; }
        [Parameter]
        public bool AllowInlineEdit { get; set; }
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        [Parameter]
        public TEntityQuery? Query { get; set; }

        [Parameter]
        public EventCallback<object> OnRowDoubleClickEventCallBack { get; set; }
        [Parameter]
        public EventCallback<TEntity> OnObjectCreatedEventCallBack { get; set; }

        private void OnRender(DataGridRenderEventArgs<TEntity> args)
        {
            if (args.FirstRender)
            {
            }
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && LoadOnDisplay)
            {
                grid.Reload();
            }

            return base.OnAfterRenderAsync(firstRender);
        }

        public async Task ResetAsync()
        {
            grid.Reset(true);
            await grid.FirstPage(true);
        }

        public async Task ReloadAsync()
        {
            ResetEdit();
            await grid.Reload();
        }

        public async Task OnRowDoubleClickEvent(DataGridRowMouseEventArgs<TEntity> e)
        {
            if (OnRowDoubleClickEventCallBack.HasDelegate)
            {
                await OnRowDoubleClickEventCallBack.InvokeAsync(e.Data.DatabaseID);
            }
        }

        private async Task LoadData(LoadDataArgs args)
        {
            try
            {
                isLoading = true;
                await Task.Yield();
                await Task.Delay(1);
                ISpecification<TEntity> query = BuildSearchQuery(args);
                var result = await EntityService.QueryAsync(query, true);

                gridEntities = result.Results;
                totalCount = result.TotalItems;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "MakeItDataGrid Error in {Function} : {ErrorMessage}", nameof(LoadData), ex.Message);

                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "An error has occured", Detail = ex.Message });
            }
            finally
            {
                _ = InvokeAsync(StateHasChanged);
                isLoading = false;
            }

            isLoading = false;
        }

        private TEntityQuery BuildSearchQuery(LoadDataArgs args)
        {
            TEntityQuery query = Query ?? new();

            if (args.Sorts?.FirstOrDefault() != null && !string.IsNullOrEmpty(args.Sorts.First().Property))
            {
                query.OrderByStrings = [ new (args.Sorts.First().Property) ];

                if (args.Sorts.First().SortOrder.HasValue)
                {
                    query.OrderByStrings[0].SortDescending = args.Sorts.First().SortOrder != SortOrder.Ascending;
                }
            }
            else
            {
                //default search

                //don't know why this is not working : typeof(TEntity).IsAssignableFrom(typeof(ITimeTrackingEntity)))
                if (typeof(TEntity).GetInterfaces().Contains(typeof(ITimeTrackingEntity)))
                {
                    query.OrderBy = [new(x => ((ITimeTrackingEntity)x).LastModificationDate, true)];
                }
            }

            string filters = string.Empty;
            if (args.Filter != null)
            {
                query.StringCriteria = args.Filter;
            }
            else if (args.Filters.Any())
            {
                query.StringCriteria = grid.ColumnsCollection.ToFilterString();
            }

            query.Skip = args.Skip;
            query.Take = args.Top;

            return (query);
        }

        private TEntity? entityToUpdate;
        private TEntity? entityToInsert;
        void ResetEdit()
        {
            entityToUpdate = null;
            entityToInsert = null;
            StateHasChanged();
        }

        async Task InsertRow()
        {
            if (!grid.IsValid)
            {
                return;
            }

            ResetEdit();

            TEntity entity = new();
            if (OnObjectCreatedEventCallBack.HasDelegate)
            {
                await OnObjectCreatedEventCallBack.InvokeAsync(entity);
            }
            entityToInsert = entity;


            await grid.InsertRow(entity);
        }

        public async Task SaveRow(TEntity entity)
        {
            await grid.UpdateRow(entity);
        }

        public async Task EditRow(TEntity entity)
        {
            if (! grid.IsValid)
            {
                return;
            }

            entityToUpdate = entity;
            await grid.EditRow(entity);
        }

        public async Task DeleteRow(TEntity entity)
        {
            bool? dialogResult = await DialogService.Confirm("Are you sure you want to delete entry ?", "Confirmation");

            if (dialogResult.GetValueOrDefault())
            {
                var dbResult = await EntityService.DeleteAsync(entity);

                if (dbResult.Result != CommandState.Success)
                {
                    NotifyError(null, "An error has occured during deletion");
                }
                else
                {
                    await grid.RefreshDataAsync();
                }
            }
        }

        public async Task CancelEdit(TEntity entity)
        {
            ResetEdit();
            grid.CancelEditRow(entity);

            await grid.Reload();
        }

        private async Task OnUpdateRow(TEntity entity)
        {
            ResetEdit();
            var dbResult = await EntityService.UpdateAsync(entity);

            if (dbResult.Result != CommandState.Success)
            {
                NotifyError(dbResult, "An error has occured during update");

                await grid.EditRow(entity);
            }
        }

        private async Task OnCreateRow(TEntity entity)
        {

            bool hasEditSucceed = false;
            try
            {
                var dbResult = await EntityService.CreateAsync(entity);
                if (dbResult.Result != CommandState.Success)
                {
                    NotifyError(dbResult, "An error has occured during creation");
                }
                else
                {
                    hasEditSucceed = true;
                }
            }
            catch (Exception exception)
            {
                NotifyError(null, $"An error has occured during creation : {exception.Message}");
                throw;
            }

            if(hasEditSucceed)
            {
                ResetEdit();
            }
        }

        private void NotifyError(CommandResult<TEntity>? dbResult, string errorMessage)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = errorMessage,
                Detail = dbResult?.Message
            });
        }
    }
}