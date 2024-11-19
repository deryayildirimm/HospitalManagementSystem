using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pusula.Training.HealthCare.Countries;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class Doctors
{
    [CreatePhoneNumberValidator]
    public string PhoneNumber { get; set; } = string.Empty;


    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = [];
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
    private Modal CreateDoctorModal { get; set; } = new();
    private Modal EditDoctorModal { get; set; } = new();
    private GetDoctorsInput Filter { get; set; }
    private List<DoctorWithNavigationPropertiesDto> SelectedDoctors { get; set; } = [];
    private IEnumerable<KeyValuePair<int, string>> Genders = [];
    
    private IReadOnlyList<LookupDto<Guid>> CitiesCollection { get; set; } = [];
    private IReadOnlyList<LookupDto<Guid>> DistrictsCollection { get; set; } = [];
    private IReadOnlyList<LookupDto<Guid>> TitlesCollection { get; set; } = [];
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; } = [];

    public Doctors()
    {
        NewDoctor = new DoctorCreateDto();
        EditingDoctor = new DoctorUpdateDto();
        Filter = new GetDoctorsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        DoctorList = [];
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await SetLookupsAsync();
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
        CanCreateDoctor = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.Doctors.Create);
        CanEditDoctor = await AuthorizationService
                        .IsGrantedAsync(HealthCarePermissions.Doctors.Edit);
        CanDeleteDoctor = await AuthorizationService
                        .IsGrantedAsync(HealthCarePermissions.Doctors.Delete);
    }
    
    private async Task SetLookupsAsync()
    {
        CitiesCollection = [.. (await DoctorsAppService.GetCityLookupAsync(new() { SkipCount = 0, MaxResultCount = 1000 })).Items];
        DistrictsCollection = [.. (await DoctorsAppService.GetDistrictLookupAsync(new() { SkipCount = 0, MaxResultCount = 1000 })).Items];
        TitlesCollection = [.. (await DoctorsAppService.GetTitleLookupAsync(new() { SkipCount = 0, MaxResultCount = 1000 })).Items];
        DepartmentsCollection = [.. (await DoctorsAppService.GetDepartmentLookupAsync(new() { SkipCount = 0, MaxResultCount = 1000 })).Items];
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

    private async Task OpenCreateDoctorModalAsync()
    {
        await GetGendersListAsync();
        await NewDoctorValidations.ClearAll();
        await ClearPhone();
        await CreateDoctorModal.Show();
    }

    private async Task OpenEditDoctorModalAsync(DoctorWithNavigationPropertiesDto input)
    {
        await GetGendersListAsync();
        await EditingDoctorValidations.ClearAll();
        var doctor = await DoctorsAppService.GetWithNavigationPropertiesAsync(input.Doctor.Id);

        EditingDoctorId = doctor.Doctor.Id;
        EditingDoctor = ObjectMapper.Map<DoctorDto, DoctorUpdateDto>(doctor.Doctor);

        await EditDoctorModal.Show();
    }


    private async Task CloseCreateDoctorModalAsync()
    {
        await ClearPhone();
        await CreateDoctorModal.Hide();
    }

    private async Task CloseEditDoctorModalAsync()
    {
        await EditDoctorModal.Hide();
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
            if (await NewDoctorValidations.ValidateAll() == false)
            {
                return;
            }
            NewDoctor.PhoneNumber = $"{PhoneNumber}";
            await DoctorsAppService.CreateAsync(NewDoctor);
            await GetDoctorsAsync();
            await CloseCreateDoctorModalAsync();
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
            if (await EditingDoctorValidations.ValidateAll() == false)
            {
                return;
            }

            await DoctorsAppService.UpdateAsync(EditingDoctor);
            await GetDoctorsAsync();
            await EditDoctorModal.Hide();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected virtual async Task OnFirstNameChangedAsync(string? firstName)
    {
        Filter.FirstName = firstName;
        await SearchAsync();
    }

    protected virtual async Task OnLastNameChangedAsync(string? lastName)
    {
        Filter.LastName = lastName;
        await SearchAsync();
    }

    protected virtual async Task OnIdentityNumberChangedAsync(string? identityNumber)
    {
        Filter.IdentityNumber = identityNumber;
        await SearchAsync();
    }

    protected virtual async Task OnBirthDateMinChangedAsync(DateTime? birthDateMin)
    {
        Filter.BirthDateMin = birthDateMin.HasValue ? birthDateMin.Value.Date : birthDateMin;
        await SearchAsync();
    }

    protected virtual async Task OnBirthDateMaxChangedAsync(DateTime? birthDateMax)
    {
        Filter.BirthDateMax = birthDateMax.HasValue ? birthDateMax.Value.Date.AddDays(1).AddSeconds(-1) : birthDateMax;
        await SearchAsync();
    }

    protected virtual async Task OnEmailChangedAsync(string? email)
    {
        Filter.Email = email;
        await SearchAsync();
    }

    protected virtual async Task OnPhoneNumberChangedAsync(string? phoneNumber)
    {
        Filter.PhoneNumber = phoneNumber;
        await SearchAsync();
    }

    protected virtual async Task OnGenderChangedAsync(EnumGender? gender)
    {
        Filter.Gender = gender;
        await SearchAsync();
    }

    protected virtual async Task OnYearOfExperienceMinChangedAsync(int? yearOfExperienceMin)
    {
        Filter.YearOfExperienceMin = yearOfExperienceMin;
        await SearchAsync();
    }
    
    private async Task OnCityChangedAsync(Guid? cityId)
    {
        Filter.CityId = cityId;

        if (cityId.HasValue)
        {
            // Fetch districts for the selected city
            DistrictsCollection = [..(await DoctorsAppService.GetDistrictLookupAsync(new() { SkipCount = 0, MaxResultCount = 1000 })).Items];
        }
        else
        {
            // Reset districts if no city is selected
            DistrictsCollection = [];
        }

        // Clear selected district if it's no longer valid
        Filter.DistrictId = null;

        await InvokeAsync(StateHasChanged);
    }

    
    private Task SelectAllItems()
    {
        AllDoctorsSelected = true;

        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllDoctorsSelected = false;
        SelectedDoctors.Clear();

        return Task.CompletedTask;
    }

    private Task ClearPhone()
    {
        PhoneNumber = string.Empty;
        return Task.CompletedTask;
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

    private Task GetGendersListAsync()
    {
        Genders = Enum.GetValues(typeof(EnumGender))
                      .Cast<EnumGender>()
                      .Select(e => new KeyValuePair<int, string>((int)e, e.ToString()))
                      .ToList();
        return Task.CompletedTask;
    }

}
