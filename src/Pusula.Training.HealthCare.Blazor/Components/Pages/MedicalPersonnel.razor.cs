using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Validators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.OpenApi.Extensions;
using Pusula.Training.HealthCare.MedicalPersonnel;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Shared;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using SortDirection = Blazorise.SortDirection;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class MedicalPersonnel
{
    [CreatePhoneNumberValidator]
    public string PhoneNumber { get; set; } = string.Empty;

    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
    private Query FilterQuery { get; set; }
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    private SfGrid<MedicalStaffWithNavigationPropertiesDto> Grid { get; set; }
    protected bool ShowAdvancedFilters { get; set; }
    private int PageSize { get; } = 10;
    private int CurrentPage { get; set; } = 1;
    private int TotalCount { get; set; }
    private string CurrentSorting { get; set; } = string.Empty;
    private bool AllMedicalStaffSelected { get; set; }
    private bool CanCreateMedicalStaff { get; set; }
    private bool CanEditMedicalStaff { get; set; }
    private bool CanDeleteMedicalStaff { get; set; }
    private MedicalStaffCreateDto NewMedicalStaff { get; set; }
    private MedicalStaffUpdateDto EditingMedicalStaff { get; set; }
    private Guid EditingMedicalStaffId { get; set; }
    private GetMedicalStaffInput Filter { get; set; }
    private List<KeyValuePair<EnumGender, string>> Genders { get; set; }
    private IReadOnlyList<LookupDto<Guid>> CitiesCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DistrictsCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; }
    
    private bool Flag { get; set; }
    private bool IsVisibleCreate { get; set; }
    private bool IsVisibleEdit { get; set; }

        
    public MedicalPersonnel()
    {
        Grid = new SfGrid<MedicalStaffWithNavigationPropertiesDto>();
        IsVisibleCreate = false;
        IsVisibleEdit = false;
        NewMedicalStaff = new MedicalStaffCreateDto();
        EditingMedicalStaff = new MedicalStaffUpdateDto();
        Filter = new GetMedicalStaffInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        Flag = false;
        FilterQuery = new Query();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await SetLookupsAsync();
        await GetGendersListAsync();
        SetFilters();
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
            await InvokeAsync(StateHasChanged);
            await Grid.EnableToolbarItemsAsync(["Delete"], false);
            await Refresh();
        }
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["MedicalStaff"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], DownloadAsExcelAsync, IconName.Download);
        Toolbar.AddButton(L["NewMedicalStaff"], OpenCreateMedicalStaffModalAsync, IconName.Plus, requiredPolicyName: HealthCarePermissions.MedicalStaff.Create);
        return ValueTask.CompletedTask;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateMedicalStaff = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.MedicalStaff.Create);
        CanEditMedicalStaff = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.MedicalStaff.Edit);
        CanDeleteMedicalStaff = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.MedicalStaff.Delete);
    }

    private async Task SetLookupsAsync()
    {
        CitiesCollection = (await MedicalStaffAppService.GetCityLookupAsync(new() { SkipCount = 0, MaxResultCount = 1000 })).Items.ToList();
        DepartmentsCollection = (await MedicalStaffAppService.GetDepartmentLookupAsync(new() { SkipCount = 0, MaxResultCount = 1000 })).Items.ToList();
    }
    
    private async Task GetMedicalStaffAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
 
        SetFilters();
        await Refresh();
    }

    private async Task ClearFilters()
    {
        Filter = new GetMedicalStaffInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting,
        };

        StateHasChanged();
        SetFilters();
        await Refresh();
    }

    public async void OnActionBegin(ActionEventArgs<MedicalStaffWithNavigationPropertiesDto> args)
    {
        
        if (args.RequestType.ToString() != "Delete" 
            // || !IsDeleteDialogVisible
            )
        {
            return;
        }
        
        if (args.RequestType.ToString() == "Paging")
        {
            return;
        }

        args.Cancel = true;
        // await DeleteConfirmDialog.ShowAsync();
        Flag = false;
        await Refresh();
    }

    public void Closed()
    {
        Flag = true;
    }

    public void RowSelectHandler(RowSelectEventArgs<MedicalStaffWithNavigationPropertiesDto> args)
    {
        var selectedRecordCount = Grid.GetSelectedRecordsAsync().Result.Count;
        if (selectedRecordCount > 0)
        {
            Grid.EnableToolbarItemsAsync(["Delete"], true);
        }
    }

    public void RowDeselectHandler(RowDeselectEventArgs<MedicalStaffWithNavigationPropertiesDto> args)
    {
        var selectedRecordCount = Grid.GetSelectedRecordsAsync().Result.Count;
        if (selectedRecordCount == 0)
        {
            Grid.EnableToolbarItemsAsync(["Delete"], false);
        }
    }

    public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
    {
        try
        {
            switch (args.Item.Text)
            {
                case "Delete":
                {
                    var selectedRecord = Grid.GetSelectedRecordsAsync().Result;

                    if (selectedRecord == null || selectedRecord.Count == 0)
                    {
                        return;
                    }

                    var ids = selectedRecord.Select(x => x.MedicalStaff.Id).ToList();

                    var confirmed = await UiMessageService.Confirm(@L["DeleteSelectedRecords", ids.Count]);
                    if (!confirmed)
                    {
                        return;
                    }

                    await MedicalStaffAppService.DeleteByIdsAsync(ids);
                    break;
                }
                case "Excel Export":
                {
                    var exportProperties = new ExcelExportProperties
                    {
                        IncludeTemplateColumn = true
                    };
                    await Grid.ExportToExcelAsync(exportProperties);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
        finally
        {
            await Refresh();
        }
    }
    

    
    private async Task DeleteMedicalStaffAsync(MedicalStaffWithNavigationPropertiesDto input)
    {
        var confirmed = await UiMessageService.Confirm($"Are you sure you want to delete {input.MedicalStaff.FirstName} {input.MedicalStaff.LastName}?");
        if (!confirmed) return;

        await MedicalStaffAppService.DeleteAsync(input.MedicalStaff.Id);
        await Refresh();
    }

    private async Task Refresh()
    {
        await Grid.Refresh();
    }

    protected virtual async Task SearchAsync()
    {
        Console.WriteLine($"Filter: {System.Text.Json.JsonSerializer.Serialize(Filter)}");
        CurrentPage = 1;
        await GetMedicalStaffAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await MedicalStaffAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HealthCare") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }
        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/medical-staff/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&FirstName={HttpUtility.UrlEncode(Filter.FirstName)}&LastName={HttpUtility.UrlEncode(Filter.LastName)}&BirthDateMin={Filter.BirthDateMin?.ToString("O")}&BirthDateMax={Filter.BirthDateMax?.ToString("O")}&IdentityNumber={HttpUtility.UrlEncode(Filter.IdentityNumber)}&Email={HttpUtility.UrlEncode(Filter.Email)}&PhoneNumber={HttpUtility.UrlEncode(Filter.PhoneNumber)}&Gender={Filter.Gender}&BirthDateMin={Filter.BirthDateMin?.ToString("O")}&CityId={Filter.CityId?.ToString("O")}&DistrictId={Filter.DistrictId?.ToString("O")}&DepartmentId={Filter.DepartmentId?.ToString("O")}", forceLoad: true);
    }

    private void OpenEditModal(MedicalStaffWithNavigationPropertiesDto doctor)
    {
        EditingMedicalStaffId = doctor.MedicalStaff.Id;
        EditingMedicalStaff = new MedicalStaffUpdateDto {
            Id = doctor.MedicalStaff.Id,
            FirstName = doctor.MedicalStaff.FirstName,
            LastName = doctor.MedicalStaff.LastName,
            BirthDate = doctor.MedicalStaff.BirthDate,
            IdentityNumber = doctor.MedicalStaff.IdentityNumber,
            CityId = doctor.MedicalStaff.CityId,
            DistrictId = doctor.MedicalStaff.DistrictId,
            DepartmentId = doctor.MedicalStaff.DepartmentId,
            Email = doctor.MedicalStaff.Email,
            Gender = doctor.MedicalStaff.Gender,
            PhoneNumber = doctor.MedicalStaff.PhoneNumber,
            StartDate = doctor.MedicalStaff.StartDate
        };
        IsVisibleEdit = true;
    }
    
    public bool EnableDistrictDropDown = false;
    protected async void ChangeCity(ChangeEventArgs<Guid?, LookupDto<Guid>> args)
    {
        EnableDistrictDropDown = true;

        if (EnableDistrictDropDown)
        {
            DistrictsCollection = [..(await MedicalStaffAppService.GetDistrictLookupAsync(args.Value, new LookupRequestDto() { SkipCount = 0, MaxResultCount = 1000 })).Items];
            StateHasChanged();
        }
        else
        {
        }
        await SearchAsync();
        Filter.DistrictId = null;
    }

    protected async void ChangeCityCreate(SelectEventArgs<LookupDto<Guid>> args)
    {
        EnableDistrictDropDown = true;

        if (EnableDistrictDropDown)
        {
            var items = (await MedicalStaffAppService.GetDistrictLookupAsync(args.ItemData.Id, new LookupRequestDto() { SkipCount = 0, MaxResultCount = 1000 })).Items;
            DistrictsCollection = items.ToList();
            StateHasChanged();
        }
    }

    private async Task OpenCreateMedicalStaffModalAsync()
    {
        IsVisibleCreate = true;
        
        await GetGendersListAsync();
        NewMedicalStaff = new MedicalStaffCreateDto();
    }
    
    private async Task CreateMedicalStaffAsync()
    {
        try
        {
            NewMedicalStaff.PhoneNumber = $"{PhoneNumber}";
            await MedicalStaffAppService.CreateAsync(NewMedicalStaff);
            await GetMedicalStaffAsync();
            CloseCreateMedicalStaffModal();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task UpdateMedicalStaffAsync()
    {
        try
        {

            await MedicalStaffAppService.UpdateAsync(EditingMedicalStaff);
            await Refresh();
            CloseEditMedicalStaffModal();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task GetGendersListAsync()
    {
        Genders = Enum.GetValues(typeof(EnumGender))
            .Cast<EnumGender>()
            .Select(s => new { Key = s, Value = s.GetDisplayName().ToString() })
            .ToDictionary(d => d.Key, d => d.Value).ToList();
    }

    private void CloseCreateMedicalStaffModal()
    {
        IsVisibleCreate = false;
    }

    private void CloseEditMedicalStaffModal()
    {
        IsVisibleEdit = false;
    }

    private async Task ClearPhone()
    {
        PhoneNumber = string.Empty;
    }


}
