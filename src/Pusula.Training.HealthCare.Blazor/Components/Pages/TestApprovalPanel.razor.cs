using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Blazor.Models.TestApprovalPanel;
using Pusula.Training.HealthCare.BloodTests;
using Pusula.Training.HealthCare.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages
{
    public partial class TestApprovalPanel
    {
        private bool CanEditBloodTest { get; set; }
        private bool IsVisibleDetailsDialog;
        private BloodTestDto? CurrentBloodTest { get; set; }

        private Dictionary<BloodTestDto, bool> expandedCards;
        private IReadOnlyList<BloodTestData> BloodTestList { get; set; } 
        private BloodTestData? BloodTestData { get; set; }

        private bool isLoading;

        public TestApprovalPanel()
        {
            isLoading = true;
            expandedCards = new();
            BloodTestList = [];
            IsVisibleDetailsDialog = false;
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            isLoading = true;
            await GetBloodTestsAsync(new GetBloodTestsInput());
            isLoading = false;
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
            CanEditBloodTest = await AuthorizationService
                .IsGrantedAsync(HealthCarePermissions.BloodTests.Edit);
        }

        private async Task GetBloodTestsAsync(GetBloodTestsInput input)
        {
            input.Status = BloodTestStatus.Requested;
            var result = await bloodTestAppService.GetListAsync(input);
            var items = result.Items.ToList();
            var bloodTestDataList = items.Select(b => new BloodTestData
            {
                BloodTestId = b.Id,
                DateCreated = b.DateCreated,
                DoctorName = $"{b.Doctor.FirstName} {b.Doctor.LastName}",
                PatientName = $"{b.Patient.FirstName} {b.Patient.LastName}",
                PatientNumber = b.Patient.PatientNumber,
                PatientId = b.Patient.IdentityNumber,
                Status = b.Status
            }).ToList();

            BloodTestList = bloodTestDataList.AsReadOnly();
            await InvokeAsync(StateHasChanged);
        }

        private void ToggleExpand(BloodTestDto bloodTest)
        {
            if (!expandedCards.ContainsKey(bloodTest))
            {
                expandedCards[bloodTest] = false; 
            }
            expandedCards[bloodTest] = !expandedCards[bloodTest];
        }

        private async Task UpdateStatusAsync()
        {
            var updateDto = new BloodTestUpdateDto
            {
                Id = CurrentBloodTest!.Id,
                Status = BloodTestStatus.PendingTest,
                DateCreated = CurrentBloodTest.DateCreated,
                DateCompleted = CurrentBloodTest.DateCompleted,
                DoctorId = CurrentBloodTest.Doctor.Id,
                PatientId = CurrentBloodTest.Patient.Id,
                TestCategoryIdList = CurrentBloodTest.BloodTestCategories!.Select(x => x.TestCategoryId).ToList()
            };
            await bloodTestAppService.UpdateAsync(updateDto);
            await CloseDetailsDialog();
        }

        private async Task CancelBloodTest()
        {
            var updateDto = new BloodTestUpdateDto
            {
                Id = CurrentBloodTest!.Id,
                Status = BloodTestStatus.Cancelled,
                DateCreated = CurrentBloodTest.DateCreated,
                DateCompleted = CurrentBloodTest.DateCompleted,
                DoctorId = CurrentBloodTest.Doctor.Id,
                PatientId = CurrentBloodTest.Patient.Id,
                TestCategoryIdList = CurrentBloodTest.BloodTestCategories!.Select(x => x.TestCategoryId).ToList()
            };
            await bloodTestAppService.UpdateAsync(updateDto);
            await CloseDetailsDialog();
        }

        private async Task OpenDetailsDialog(BloodTestData patient)
        {
            CurrentBloodTest = await bloodTestAppService.GetAsync(patient.BloodTestId);
            IsVisibleDetailsDialog = true;
        }

        private async Task CloseDetailsDialog()
        {
            IsVisibleDetailsDialog = false;
            CurrentBloodTest = null!;
            await GetBloodTestsAsync(new GetBloodTestsInput());
        }
    }
}