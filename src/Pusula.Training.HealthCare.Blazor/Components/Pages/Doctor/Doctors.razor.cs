using Blazorise;
using Blazorise.DataGrid;
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
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Popups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using SortDirection = Blazorise.SortDirection;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages.Doctor;

public partial class Doctors
{
    [CreatePhoneNumberValidator]
    public string PhoneNumber { get; set; } = string.Empty;

    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }
    private IReadOnlyList<DoctorWithNavigationPropertiesDto> DoctorList { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private int TotalCount { get; set; }
    private string CurrentSorting { get; set; } = string.Empty;
    private bool AllDoctorsSelected { get; set; }
    private bool CanCreateDoctor { get; set; }
    private bool CanEditDoctor { get; set; }
    private bool CanDeleteDoctor { get; set; }
    private DoctorCreateDto NewDoctor { get; set; }
    private DoctorUpdateDto EditingDoctor { get; set; }
    private Validations NewDoctorValidations { get; set; } = new();
    private Validations EditingDoctorValidations { get; set; } = new();
    private Guid EditingDoctorId { get; set; }
    private SfDialog CreateDoctorModal;
    private SfDialog EditDoctorModal;
    private GetDoctorsInput Filter { get; set; }
    private List<DoctorWithNavigationPropertiesDto> SelectedDoctors { get; set; } = new();
    private List<KeyValuePair<EnumGender, string>> Genders { get; set; }
    private IReadOnlyList<LookupDto<Guid>> CitiesCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DistrictsCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> TitlesCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; }

    private bool IsVisibleCreate { get; set; }
    private bool IsVisibleEdit { get; set; }

        
    public Doctors()
    {
        
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
        DoctorList = new List<DoctorWithNavigationPropertiesDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await SetLookupsAsync();
        await GetGendersListAsync();
        await SearchAsync();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Doctors"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], DownloadAsExcelAsync, IconName.Download);
        Toolbar.AddButton(L["NewDoctor"], OpenCreateDoctorModalAsync, IconName.Add, requiredPolicyName: HealthCarePermissions.Doctors.Create);
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
        CitiesCollection = (await DoctorsAppService.GetCityLookupAsync(new() { SkipCount = 0, MaxResultCount = 1000 })).Items.ToList();
        
        TitlesCollection = (await DoctorsAppService.GetTitleLookupAsync(new() { SkipCount = 0, MaxResultCount = 1000 })).Items.ToList();
        DepartmentsCollection = (await DoctorsAppService.GetDepartmentLookupAsync(new() { SkipCount = 0, MaxResultCount = 1000 })).Items.ToList();
    }
    
    public async Task PageChangingHandler(GridPageChangingEventArgs args)
    {
        CurrentPage = args.CurrentPage;
        await GetDoctorsAsync();
    }

    public async Task PageChangedHandler(GridPageChangedEventArgs args)
    {
        CurrentPage = args.CurrentPage;
        await GetDoctorsAsync();    

    }
    private async Task GetDoctorsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;

        var result = await DoctorsAppService.GetListAsync(Filter);
        DoctorList = result.Items;
        TotalCount = (int)result.TotalCount;

        await ClearSelection();
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
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }
        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/patients/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&FirstName={HttpUtility.UrlEncode(Filter.FirstName)}&LastName={HttpUtility.UrlEncode(Filter.LastName)}&BirthDateMin={Filter.BirthDateMin?.ToString("O")}&BirthDateMax={Filter.BirthDateMax?.ToString("O")}&IdentityNumber={HttpUtility.UrlEncode(Filter.IdentityNumber)}&Email={HttpUtility.UrlEncode(Filter.Email)}&PhoneNumber={HttpUtility.UrlEncode(Filter.PhoneNumber)}&Gender={Filter.Gender}", forceLoad: true);
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

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<DoctorWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;
        await GetDoctorsAsync();
        await InvokeAsync(StateHasChanged);
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
    
    private async Task DeleteDoctorAsync(DoctorWithNavigationPropertiesDto input)
    {
        var confirmed = await UiMessageService.Confirm($"Are you sure you want to delete {input.Doctor.FirstName} {input.Doctor.LastName}?");
        if (!confirmed) return;

        await DoctorsAppService.DeleteAsync(input.Doctor.Id);
        await GetDoctorsAsync();
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
            await GetDoctorsAsync();
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

    private Task SelectAllItems()
    {
        AllDoctorsSelected = true;

        return Task.CompletedTask;
    }


    private async Task ClearSelection()
    {
        SelectedDoctors.Clear();
        AllDoctorsSelected = false;
    }
    
    private Task SelectedDoctorRowsChanged()
    {
        if (SelectedDoctors.Count != PageSize)
        {
            AllDoctorsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedDoctorsAsync()
    {
        var message = AllDoctorsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedDoctors.Count].Value;

        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllDoctorsSelected)
        {
            await DoctorsAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await DoctorsAppService.DeleteByIdsAsync(SelectedDoctors.Select(x => x.Doctor.Id).ToList());
        }

        SelectedDoctors.Clear();
        AllDoctorsSelected = false;

        await GetDoctorsAsync();
    }

    private void CloseCreateDoctorModal()
    {
        IsVisibleCreate = false;
    }

    private void CloseEditDoctorModal()
    {
        IsVisibleEdit = false;
    }

    private async Task ClearPhone()
    {
        PhoneNumber = string.Empty;
    }

}