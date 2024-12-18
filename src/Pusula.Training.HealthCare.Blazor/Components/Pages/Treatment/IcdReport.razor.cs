using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Treatment.Icds;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Grids;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages.Treatment;

public partial class IcdReport
{
    private DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-1);
    private DateTime EndDate { get; set; } = DateTime.Now;
    
    private Query FilterQuery { get; set; }
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    private SfGrid<IcdReportDto> Grid { get; set; }
    private int PageSize { get; } = 2;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private GetIcdReportInput Filter { get; set; }
    private bool Flag { get; set; }
    private List<IcdReportDto>? IcdReportList;
    
    public IcdReport()
    {
        Grid = new SfGrid<IcdReportDto>();
        Filter = new GetIcdReportInput
        {
            MaxResultCount = PageSize,
            SkipCount = 0
        };
        Flag = false;
        FilterQuery = new Query();
    }

    protected override async Task OnInitializedAsync()
    {
        SetFilters();
    }
    
    private async Task FetchReport()
    {
        //IcdReportList = await ExaminationsAppService.GetIcdReportAsync(StartDate, EndDate);
    }

    private void SetFilters()
    {
        FilterQuery.Queries.Params = new Dictionary<string, object>();
        FilterQuery.Queries.Params.Add("Filter", Filter);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SetBreadcrumbItemsAsync();
            await SetToolbarItemsAsync();
            await Refresh();
            await InvokeAsync(StateHasChanged);
        }
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], DownloadAsExcelAsync, IconName.Download);
        return ValueTask.CompletedTask;
    }
    private async Task GetIcdReportAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        
        SetFilters();
        await Refresh();
    }
    private async Task ClearFilters()
    {
        Filter = new GetIcdReportInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting,
        };

        StateHasChanged();
        SetFilters();
        await Refresh();
    }

    public async void OnActionBegin(ActionEventArgs<IcdDto> args)
    {
        if (args.RequestType.ToString() == "Paging")
        {
            return;
        }
        Flag = false;
        await Refresh();
    }

    public void Closed()
    {
        Flag = true;
    }

    private async Task DownloadAsExcelAsync()
    {
        //var token = (await IcdAppService.GetDownloadTokenAsync()).Token;
        //var remoteService =
        //    await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HealthCare") ??
        //    await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        //var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        //if (!culture.IsNullOrEmpty())
        //{
        //    culture = "&culture=" + culture;
        //}

        //await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        //NavigationManager.NavigateTo(
        //    $"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/appointment/as-excel-file?DownloadToken={token}&FilterText={culture}&Name=",
        //    forceLoad: true);
    }

    private async Task Refresh()
    {
        await Grid.Refresh();
    }
}