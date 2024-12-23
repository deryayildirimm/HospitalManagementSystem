using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Validators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.OpenApi.Extensions;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Shared;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.Data;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Volo.Abp;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages.Doctor;

public partial class Doctors
{
    [CreatePhoneNumberValidator]
    public string PhoneNumber { get; set; } = string.Empty;

    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
    private Query FilterQuery { get; set; }
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    private SfGrid<DoctorWithNavigationPropertiesDto> Grid { get; set; }
    protected bool ShowAdvancedFilters { get; set; }
    private int PageSize { get; } = 10;
    private int CurrentPage { get; set; } = 1;
    private int TotalCount { get; set; }
    private string CurrentSorting { get; set; } = string.Empty;
    private bool AllDoctorsSelected { get; set; }
    private bool CanCreateDoctor { get; set; }
    private bool CanEditDoctor { get; set; }
    private bool CanDeleteDoctor { get; set; }
    private DoctorCreateDto NewDoctor { get; set; }
    private DoctorUpdateDto EditingDoctor { get; set; }
    private Guid EditingDoctorId { get; set; }
    private GetDoctorsInput Filter { get; set; }
    private List<KeyValuePair<EnumGender, string>> Genders { get; set; }
    private IReadOnlyList<LookupDto<Guid>> CitiesCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DistrictsCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> TitlesCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; }

    private bool Flag { get; set; }
    private bool IsVisibleCreate { get; set; }
    private bool IsVisibleEdit { get; set; }

        
    public Doctors()
    {
        Grid = new SfGrid<DoctorWithNavigationPropertiesDto>();
        IsVisibleCreate = false;
        IsVisibleEdit = false;
        NewDoctor = new DoctorCreateDto();
        EditingDoctor = new DoctorUpdateDto();
        Filter = new GetDoctorsInput
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
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], DownloadAsExcelAsync, IconName.Download);
        Toolbar.AddButton(L["NewDoctor"], OpenCreateDoctorModalAsync, IconName.Plus, requiredPolicyName: HealthCarePermissions.Doctors.Create);
        return ValueTask.CompletedTask;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateDoctor = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.Doctors.Create);
        CanEditDoctor = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.Doctors.Edit);
        CanDeleteDoctor = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.Doctors.Delete);
    }
    private async Task SetLookupsAsync()
    {
        try
        {
            CitiesCollection =
                (await LookupAppService.GetCityLookupAsync(new LookupRequestDto
                    { MaxResultCount = 1000 }))
                .Items;

            DepartmentsCollection =
                (await LookupAppService.GetDepartmentLookupAsync(new LookupRequestDto
                    { MaxResultCount = 1000 }))
                .Items;

            TitlesCollection = (await LookupAppService.GetTitleLookupAsync(new LookupRequestDto
                    { MaxResultCount = 1000 }))
                .Items;
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
        }
    }
    
    private async Task GetDoctorsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        
        SetFilters();
        await Refresh();
    }

    private async Task ClearFilters()
    {
        Filter = new GetDoctorsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting,
        };

        StateHasChanged();
        SetFilters();
        await Refresh();
    }

    public async void OnActionBegin(ActionEventArgs<DoctorWithNavigationPropertiesDto> args)
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

    public void RowSelectHandler(RowSelectEventArgs<DoctorWithNavigationPropertiesDto> args)
    {
        var selectedRecordCount = Grid.GetSelectedRecordsAsync().Result.Count;
        if (selectedRecordCount > 0)
        {
            Grid.EnableToolbarItemsAsync(["Delete"], true);
        }
    }

    public void RowDeselectHandler(RowDeselectEventArgs<DoctorWithNavigationPropertiesDto> args)
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

                    var ids = selectedRecord.Select(x => x.Doctor.Id).ToList();

                    var confirmed = await UiMessageService.Confirm(@L["DeleteSelectedRecords", ids.Count]);
                    if (!confirmed)
                    {
                        return;
                    }

                    await DoctorsAppService.DeleteByIdsAsync(ids);
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
    

    
    private async Task DeleteDoctorAsync(DoctorWithNavigationPropertiesDto input)
    {
        var confirmed = await UiMessageService.Confirm($"Are you sure you want to delete {input.Doctor.FirstName} {input.Doctor.LastName}?");
        if (!confirmed) return;

        await DoctorsAppService.DeleteAsync(input.Doctor.Id);
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
        await GetDoctorsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await DoctorsAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HealthCare") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }
        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/doctors/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&FirstName={HttpUtility.UrlEncode(Filter.FirstName)}&LastName={HttpUtility.UrlEncode(Filter.LastName)}&BirthDateMin={Filter.BirthDateMin?.ToString("O")}&BirthDateMax={Filter.BirthDateMax?.ToString("O")}&IdentityNumber={HttpUtility.UrlEncode(Filter.IdentityNumber)}&Email={HttpUtility.UrlEncode(Filter.Email)}&PhoneNumber={HttpUtility.UrlEncode(Filter.PhoneNumber)}&Gender={Filter.Gender}&BirthDateMin={Filter.BirthDateMin?.ToString("O")}&CityId={Filter.CityId?.ToString("O")}&DistrictId={Filter.DistrictId?.ToString("O")}&TitleId={Filter.TitleId?.ToString("O")}&DepartmentId={Filter.DepartmentId?.ToString("O")}", forceLoad: true);
    }

    private void OpenEditModal(DoctorWithNavigationPropertiesDto doctor)
    {
        EditingDoctorId = doctor.Doctor.Id;
        EditingDoctor = new DoctorUpdateDto {
            Id = doctor.Doctor.Id,
            FirstName = doctor.Doctor.FirstName,
            LastName = doctor.Doctor.LastName,
            BirthDate = doctor.Doctor.BirthDate,
            IdentityNumber = doctor.Doctor.IdentityNumber,
            CityId = doctor.Doctor.CityId,
            DistrictId = doctor.Doctor.DistrictId,
            DepartmentId = doctor.Doctor.DepartmentId,
            TitleId = doctor.Doctor.TitleId,
            Email = doctor.Doctor.Email,
            Gender = doctor.Doctor.Gender,
            PhoneNumber = doctor.Doctor.PhoneNumber,
            StartDate = doctor.Doctor.StartDate
        };
        IsVisibleEdit = true;
    }
    
    public bool EnableDistrictDropDown = false;
    protected async void ChangeCity(ChangeEventArgs<Guid?, LookupDto<Guid>> args)
    {
        EnableDistrictDropDown = true;

        if (EnableDistrictDropDown)
        {
            DistrictsCollection = [..(await DoctorsAppService.GetDistrictLookupAsync(args.Value, new LookupRequestDto() { SkipCount = 0, MaxResultCount = 1000 })).Items];
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
            var items = (await DoctorsAppService.GetDistrictLookupAsync(args.ItemData.Id, new LookupRequestDto() { SkipCount = 0, MaxResultCount = 1000 })).Items;
            DistrictsCollection = items.ToList();
            StateHasChanged();
        }
    }

    private async Task OpenCreateDoctorModalAsync()
    {
        IsVisibleCreate = true;
        
        await GetGendersListAsync();
        NewDoctor = new DoctorCreateDto();
    }

    private async Task CreateDoctorAsync()
    {
        try
        {
            NewDoctor.PhoneNumber = $"{PhoneNumber}";
            await DoctorsAppService.CreateAsync(NewDoctor);
            await GetDoctorsAsync();
            CloseCreateDoctorModal();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task UpdateDoctorAsync()
    {
        try
        {
            await DoctorsAppService.UpdateAsync(EditingDoctor);
            await Refresh();
            CloseEditDoctorModal();
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
    private void CloseCreateDoctorModal()
    {
        IsVisibleCreate = false;
    }

    private void CloseEditDoctorModal()
    {
        IsVisibleEdit = false;
    }
}