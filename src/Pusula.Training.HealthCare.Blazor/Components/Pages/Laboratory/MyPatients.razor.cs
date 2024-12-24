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

namespace Pusula.Training.HealthCare.Blazor.Components.Pages.Laboratory;

public partial class MyPatients
{
    private IReadOnlyList<PatientDto> Patients { get; set; }
    private IReadOnlyList<TestCategoryDto>? Categories { get; set; }
    private List<Guid>? SelectedCategoryIds { get; set; }
    private PatientDto? Patient { get; set; }
    private GetPatientsInput Filter { get; set; } 
    private bool CanCreateBloodTest { get; set; }
    private bool Disabled => SelectedCategoryIds?.Count == 0;
    private bool IsVisibleChooseTestDialog;
    private Guid DoctorId { get; set; }
    private bool isLoading;

    public MyPatients()
    {
        Filter = new();
        isLoading = true;
        Patients = [];
        IsVisibleChooseTestDialog = false;
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        isLoading = true;
        await GetPatientsAsync();
        await GetDoctor();
        isLoading = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InvokeAsync(StateHasChanged);
        }
    }

    protected void NavigateToDetail(PatientDto patient)
    {
        NavigationManager.NavigateTo($"/doctor/my-patients/reports/{patient.PatientNumber}");
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateBloodTest = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.BloodTests.Create);
    }

    private async Task GetPatientsAsync()
    {
        var result = await PatientsAppService.GetListAsync(Filter);
        Patients = result.Items;

        await ClearSelectedCategories();
    }

    private async Task OpenChooseTestDialog(PatientDto input)
    {
        var getTestCategoriesInput = new GetTestCategoriesInput();
        var categoriesResult = await testCategoryAppService.GetListAsync(getTestCategoriesInput);
        Categories = categoriesResult.Items;

        SelectedCategoryIds = [];

        Patient = await PatientsAppService.GetAsync(input.Id);
        IsVisibleChooseTestDialog = true;
    }

    private async Task CloseDialog()
    {
        await ClearSelectedCategories();
        IsVisibleChooseTestDialog = false;
    }

    private async Task ChooseBloodTests()
    {
        try
        {
            var date = DateTime.Now;
            await BloodTestAppService.CreateAsync(new BloodTestCreateDto
            {
                DoctorId = DoctorId,
                PatientId = Patient!.Id,
                Status = BloodTestStatus.Requested,
                DateCreated = date,
                TestCategoryIdList = SelectedCategoryIds!
            });

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
            var doctors = (await DoctorAppService.GetListAsync(new GetDoctorsInput())).Items;

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
