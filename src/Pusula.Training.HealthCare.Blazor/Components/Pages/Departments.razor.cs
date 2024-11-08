using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Permissions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;



namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class Departments
{

    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = [];
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }
    private IReadOnlyList<DepartmentDto> DepartmentList { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }
    private bool CanCreateDepartment { get; set; }
    private bool CanEditDepartment { get; set; }
    private bool CanDeleteDepartment { get; set; }
    private DepartmentCreateDto NewDepartment { get; set; }
    private Validations NewDepartmentValidations { get; set; } = new();
    private DepartmentUpdateDto EditingDepartment { get; set; }
    private Validations EditingDepartmentValidations { get; set; } = new();
    private Guid EditingDepartmentId { get; set; }
    private Modal CreateDepartmentModal { get; set; } = new();
    private Modal EditDepartmentModal { get; set; } = new();
    private GetDepartmentsInput Filter { get; set; }
    private DataGridEntityActionsColumn<DepartmentDto> EntityActionsColumn { get; set; } = new();
    protected string SelectedCreateTab = "department-create-tab";
    protected string SelectedEditTab = "department-edit-tab";



    private List<DepartmentDto> SelectedDepartments { get; set; } = [];
    private bool AllDepartmentsSelected { get; set; }

    public Departments()
    {
        NewDepartment = new DepartmentCreateDto();
        EditingDepartment = new DepartmentUpdateDto();
        Filter = new GetDepartmentsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        DepartmentList = [];


    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();

    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {

            await SetBreadcrumbItemsAsync();
            await SetToolbarItemsAsync();
            await InvokeAsync(StateHasChanged);
        }
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Departments"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], DownloadAsExcelAsync, IconName.Download);

        Toolbar.AddButton(L["NewDepartment"], OpenCreateDepartmentModalAsync, IconName.Add, requiredPolicyName: HealthCarePermissions.Departments.Create);

        return ValueTask.CompletedTask;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateDepartment = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.Departments.Create);
        CanEditDepartment = await AuthorizationService
                        .IsGrantedAsync(HealthCarePermissions.Departments.Edit);
        CanDeleteDepartment = await AuthorizationService
                        .IsGrantedAsync(HealthCarePermissions.Departments.Delete);


    }

    private async Task GetDepartmentsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;

        var result = await DepartmentsAppService.GetListAsync(Filter);
        DepartmentList = result.Items;
        TotalCount = (int)result.TotalCount;

        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetDepartmentsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await DepartmentsAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HealthCare") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }
        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/departments/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<DepartmentDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;
        await GetDepartmentsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateDepartmentModalAsync()
    {
        NewDepartment = new DepartmentCreateDto
        {


        };

        SelectedCreateTab = "department-create-tab";


        await NewDepartmentValidations.ClearAll();
        await CreateDepartmentModal.Show();
    }

    private async Task CloseCreateDepartmentModalAsync()
    {
        NewDepartment = new DepartmentCreateDto
        {


        };
        await CreateDepartmentModal.Hide();
    }

    private async Task OpenEditDepartmentModalAsync(DepartmentDto input)
    {
        SelectedEditTab = "department-edit-tab";


        var department = await DepartmentsAppService.GetAsync(input.Id);

        EditingDepartmentId = department.Id;
        EditingDepartment = ObjectMapper.Map<DepartmentDto, DepartmentUpdateDto>(department);

        await EditingDepartmentValidations.ClearAll();
        await EditDepartmentModal.Show();
    }

    private async Task DeleteDepartmentAsync(DepartmentDto input)
    {
        await DepartmentsAppService.DeleteAsync(input.Id);
        await GetDepartmentsAsync();
    }

    private async Task CreateDepartmentAsync()
    {
        try
        {
            if (await NewDepartmentValidations.ValidateAll() == false)
            {
                return;
            }

            await DepartmentsAppService.CreateAsync(NewDepartment);
            await GetDepartmentsAsync();
            await CloseCreateDepartmentModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditDepartmentModalAsync()
    {
        await EditDepartmentModal.Hide();
    }

    private async Task UpdateDepartmentAsync()
    {
        try
        {
            if (await EditingDepartmentValidations.ValidateAll() == false)
            {
                return;
            }

            await DepartmentsAppService.UpdateAsync(EditingDepartmentId, EditingDepartment);
            await GetDepartmentsAsync();
            await EditDepartmentModal.Hide();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void OnSelectedCreateTabChanged(string name)
    {
        SelectedCreateTab = name;
    }

    private void OnSelectedEditTabChanged(string name)
    {
        SelectedEditTab = name;
    }

    protected virtual async Task OnNameChangedAsync(string? name)
    {
        Filter.Name = name;
        await SearchAsync();
    }


    private Task SelectAllItems()
    {
        AllDepartmentsSelected = true;

        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllDepartmentsSelected = false;
        SelectedDepartments.Clear();

        return Task.CompletedTask;
    }

    private Task SelectedDepartmentRowsChanged()
    {
        if (SelectedDepartments.Count != PageSize)
        {
            AllDepartmentsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedDepartmentsAsync()
    {
        var message = AllDepartmentsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedDepartments.Count].Value;

        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllDepartmentsSelected)
        {
            await DepartmentsAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await DepartmentsAppService.DeleteByIdsAsync(SelectedDepartments.Select(x => x.Id).ToList());
        }

        SelectedDepartments.Clear();
        AllDepartmentsSelected = false;

        await GetDepartmentsAsync();
    }


}
