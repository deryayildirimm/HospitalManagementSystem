using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.BloodTests;
using Pusula.Training.HealthCare.BloodTests.Category;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;


namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class MyPatients
{
    private GetDoctorsInput DoctorsInput { get; set; } = new();
    private IReadOnlyList<PatientDto> Patients { get; set; }
    private IReadOnlyList<TestCategoryDto>? Categories { get; set; }
    private List<Guid>? SelectedCategoryIds { get; set; } 
    private PatientDto? Patient { get; set; }
    private GetPatientsInput Filter { get; set; } = new();
    private bool CanCreateBloodTest { get; set; }
    private bool Disabled => SelectedCategoryIds?.Count == 0; 
    private bool IsDialogVisible = false;
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private int TotalCount { get; set; }
    private string CurrentSorting { get; set; } = string.Empty;
    private DoctorWithNavigationPropertiesDto DoctorWithNavigation { get; set; }
    private string DoctorNameInfo { get; set; }
    private Guid DoctorId { get; set; } 

    public MyPatients()
    {
        Filter = new GetPatientsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        Patients = [];
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await GetPatientsAsync();
        await GetDoctor();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateBloodTest = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.BloodTests.Create);
    }

    private async Task GetPatientsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;

        var result = await PatientsAppService.GetListAsync(Filter);
        Patients = result.Items;
        TotalCount = (int)result.TotalCount;

        await ClearSelectedCategories();
    }

    private async Task OpenDialog(PatientDto input)
    {
        var getTestCategoriesInput = new GetTestCategoriesInput();
        var categoriesResult = await testCategoryAppService.GetListAsync(getTestCategoriesInput);
        Categories = categoriesResult.Items;

        SelectedCategoryIds = [];

        Patient = await PatientsAppService.GetAsync(input.Id);
        IsDialogVisible = true;
    }

    private async Task CloseDialog()
    {
        await ClearSelectedCategories();
        IsDialogVisible = false;
    }

    private async Task ChooseBloodTests()
    {
        try
        {
            var date = DateTime.Now;
            foreach (var items in SelectedCategoryIds!)
            {
                var bloodTest = await BloodTestAppService.CreateAsync(new BloodTestCreateDto
                {
                    DoctorId = DoctorId,
                    PatientId = Patient!.Id,
                    TestCategoryId = items,
                    Status = BloodTestStatus.Requested,
                    DateCreated = date
                });
            }
                await GetPatientsAsync();
                await CloseDialog();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void ToggleCategorySelection(Guid categoryId)
    {
        if (SelectedCategoryIds!.Contains(categoryId))
        {
            SelectedCategoryIds.Remove(categoryId); 
        }
        else
        {
            SelectedCategoryIds.Add(categoryId); 
        }
    }

    private async Task ClearSelectedCategories()
    {
        SelectedCategoryIds = [];
        await Task.CompletedTask;
    }

    private string GetCardCssClass(Guid categoryId)
    {
        return SelectedCategoryIds!.Contains(categoryId) ? "custom-card selected-card" : "custom-card";
    }

    private async Task GetDoctor()
    {
        try
        {
            var doctors = (await DoctorAppService.GetListAsync(DoctorsInput)).Items;

            if (doctors.Any())
            {
                DoctorId = doctors[0].Doctor.Id;
            }
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
    }
}
