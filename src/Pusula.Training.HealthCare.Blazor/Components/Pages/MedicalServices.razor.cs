using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Blazor.Models;
using Syncfusion.Blazor.DropDowns;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;


namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class MedicalServices : HealthCareComponentBase
{
    #pragma warning disable BL0005
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = [];
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }
    private IReadOnlyList<MedicalServiceDto> MedicalServiceList { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }
    private bool ToastVisible { get; set; } = false;
    private string EditStatus { get; set; } = string.Empty;
    private string EditMessage { get; set; } = string.Empty;
    private bool CanCreateMedicalService { get; set; }
    private bool CanEditMedicalService { get; set; }
    private bool CanDeleteMedicalService { get; set; }
    private bool IsServiceListLoading { get; set; } = true;
    private Visibility ProgressVisibility => IsServiceListLoading ? Visibility.Visible : Visibility.Invisible;
    private MedicalServiceCreateDto NewMedicalService { get; set; }
    private Validations NewMedicalServiceValidations { get; set; } = new();
    private MedicalServiceUpdateDto EditingMedicalService { get; set; }
    private Validations EditingMedicalServiceValidations { get; set; } = new();
    private Guid EditingMedicalServiceId { get; set; }
    private Modal CreateMedicalServiceModal { get; set; } = new();
    private Modal EditMedicalServiceModal { get; set; } = new();
    private GetMedicalServiceInput Filter { get; set; }
    private DataGridEntityActionsColumn<MedicalServiceDto> EntityActionsColumn { get; set; } = new();
    protected string SelectedCreateTab = "medical-service-create-tab";
    protected string SelectedEditTab = "medical-service-edit-tab";
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; } = [];
    private List<SelectionItem> DepartmentsCreateSelectionItems { get; set; } = [];
    private List<SelectionItem> DepartmentsUpdateSelectionItems { get; set; } = [];
    private List<MedicalServiceDto> SelectedMedicalServices { get; set; } = [];
    private SfMultiSelect<string[], SelectionItem> CreateDepartmentDropdown { get; set; }
    private SfMultiSelect<string[], SelectionItem> UpdateDepartmentDropdown { get; set; }

    private List<string> SelectedDepartments { get; set; }
    private bool AllMedicalServicesSelected { get; set; }

    public MedicalServices()
    {
        NewMedicalService = new MedicalServiceCreateDto();
        EditingMedicalService = new MedicalServiceUpdateDto();
        Filter = new GetMedicalServiceInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        CreateDepartmentDropdown = new SfMultiSelect<string[], SelectionItem>();
        UpdateDepartmentDropdown = new SfMultiSelect<string[], SelectionItem>();
        MedicalServiceList = [];
        DepartmentsCollection = new List<LookupDto<Guid>>();
        SelectedDepartments = [];
    }

    protected override async Task OnInitializedAsync()
    {
        IsServiceListLoading = true;
        await SetPermissionsAsync();
        await GetDepartmentCollectionLookupAsync();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["MedicalServices"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], DownloadAsExcelAsync, IconName.Download);

        Toolbar.AddButton(L["NewMedicalService"], OpenCreateMedicalServiceModalAsync, IconName.Add,
            requiredPolicyName: HealthCarePermissions.MedicalServices.Create);

        return ValueTask.CompletedTask;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateMedicalService = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.MedicalServices.Create);
        CanEditMedicalService = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.MedicalServices.Edit);
        CanDeleteMedicalService = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.MedicalServices.Delete);
    }

    private async Task GetMedicalServicesAsync()
    {
        IsServiceListLoading = true;
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await MedicalServicesAppService.GetListAsync(Filter);
        MedicalServiceList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
        IsServiceListLoading = false;
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetMedicalServicesAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await MedicalServicesAppService.GetDownloadTokenAsync()).Token;
        var remoteService =
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HealthCare") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo(
            $"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/medical-service/as-excel-file?DownloadToken={token}",
            forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<MedicalServiceDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;
        await GetMedicalServicesAsync();
        await InvokeAsync(StateHasChanged);
    }
    
    public void OnDepartmentValueChange(MultiSelectChangeEventArgs<string[]> args)
    {
        if (args.Value == null)
        {
            return;
        }
        
        SelectedDepartments.AddRange(args.Value);
    }

    private async Task OpenCreateMedicalServiceModalAsync()
    {
        NewMedicalService = new MedicalServiceCreateDto
        {
            ServiceCreatedAt = DateTime.Now,
        };
        
        SelectedCreateTab = "medical-service-create-tab";

        await NewMedicalServiceValidations.ClearAll();
        await CreateMedicalServiceModal.Show();
    }

    private async Task CloseCreateMedicalServiceModalAsync()
    {
        NewMedicalService = new MedicalServiceCreateDto
        {
            ServiceCreatedAt = DateTime.Now,
        };
        
        await InvokeAsync(StateHasChanged);
        await CreateMedicalServiceModal.Hide();
    }

    private async Task OpenEditMedicalServiceModalAsync(MedicalServiceDto input)
    {
        SelectedEditTab = "medical-service-edit-tab";

        var service = await MedicalServicesAppService.GetAsync(input.Id);

        EditingMedicalServiceId = service.Id;
        EditingMedicalService = ObjectMapper.Map<MedicalServiceDto, MedicalServiceUpdateDto>(service);

        EditingMedicalService.DepartmentNames.Clear();
        await EditingMedicalServiceValidations.ClearAll();
        await EditMedicalServiceModal.Show();
    }

    private async Task DeleteMedicalServiceAsync(MedicalServiceDto input)
    {
        await MedicalServicesAppService.DeleteAsync(input.Id);
        await GetMedicalServicesAsync();
    }

    private async Task CreateMedicalServiceAsync()
    {
        try
        {
            if (await NewMedicalServiceValidations.ValidateAll() == false)
            {
                return;
            }
            
            NewMedicalService.DepartmentNames.Clear();
            NewMedicalService.DepartmentNames.AddRange(SelectedDepartments);

            await MedicalServicesAppService.CreateAsync(NewMedicalService);

            ToastVisible = true;
            EditStatus = @L["Created"];
            EditMessage = @L["CreatedSuccess"];

            NewMedicalService.DepartmentNames.Clear();
           

            await GetMedicalServicesAsync();
            await CloseCreateMedicalServiceModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            SelectedDepartments.Clear();
            CreateDepartmentDropdown.Text = null;
            CreateDepartmentDropdown.Value = [];
        }
    }

    private async Task UpdateMedicalServiceAsync()
    {
        try
        {
            if (await EditingMedicalServiceValidations.ValidateAll() == false)
            {
                return;
            }

            EditingMedicalService.DepartmentNames.Clear();
            EditingMedicalService.DepartmentNames.AddRange(SelectedDepartments);
            
            await MedicalServicesAppService.UpdateAsync(EditingMedicalServiceId, EditingMedicalService);

            ToastVisible = true;
            EditStatus = @L["Updated"];
            EditMessage =@L["UpdatedSuccess"];
            
            await GetMedicalServicesAsync();
            await CloseEditMedicalServiceModalAsync();
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
        }finally
        {
            EditingMedicalService.DepartmentNames.Clear();
            SelectedDepartments.Clear();
            UpdateDepartmentDropdown.Text = null;
            UpdateDepartmentDropdown.Value = [];
        }
    }

    private async Task CloseEditMedicalServiceModalAsync()
    {
        EditingMedicalService = new MedicalServiceUpdateDto();
        await EditMedicalServiceModal.Hide();
    }

    protected virtual async Task OnStartTimeMinChangedAsync(DateTime? serviceTimeMin)
    {
        Filter.ServiceDateMin = serviceTimeMin.HasValue ? serviceTimeMin.Value.Date : serviceTimeMin;
        await SearchAsync();
    }

    protected virtual async Task OnStartTimeMaxChangedAsync(DateTime? serviceTimeMax)
    {
        Filter.ServiceDateMax = serviceTimeMax.HasValue
            ? serviceTimeMax.Value.Date.AddDays(1).AddSeconds(-1)
            : serviceTimeMax;
        await SearchAsync();
    }

    private async Task GetDepartmentCollectionLookupAsync(string? newValue = null)
    {
        DepartmentsCollection =
            (await LookupAppService.GetDepartmentLookupAsync(new LookupRequestDto { Filter = newValue }))
            .Items;

        DepartmentsCreateSelectionItems = DepartmentsCollection.Select(department => new SelectionItem
        {
            Id = department.Id,
            DisplayName = department.DisplayName,
            IsSelected = false
        }).ToList();

        DepartmentsUpdateSelectionItems = DepartmentsCollection.Select(department => new SelectionItem
        {
            Id = department.Id,
            DisplayName = department.DisplayName,
            IsSelected = false
        }).ToList();
    }

    private Task SelectAllItems()
    {
        AllMedicalServicesSelected = true;

        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllMedicalServicesSelected = false;
        SelectedMedicalServices.Clear();


        return Task.CompletedTask;
    }

    private Task SelectedMedicalServiceRowsChanged()
    {
        if (SelectedMedicalServices.Count != PageSize)
        {
            AllMedicalServicesSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedMedicalServicesAsync()
    {
        var message = AllMedicalServicesSelected
            ? L["DeleteAllRecords"].Value
            : L["DeleteSelectedRecords", SelectedMedicalServices.Count].Value;

        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllMedicalServicesSelected)
        {
            await MedicalServicesAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await MedicalServicesAppService.DeleteByIdsAsync(SelectedMedicalServices.Select(x => x.Id).ToList());
        }

        SelectedMedicalServices.Clear();
        AllMedicalServicesSelected = false;

        await GetMedicalServicesAsync();
    }
    
    private void NavigateToDetail(MedicalServiceDto service)
    {
        NavigationManager.NavigateTo($"/appointments/definitions/medical-service/{service.Id}");
    }
    
}