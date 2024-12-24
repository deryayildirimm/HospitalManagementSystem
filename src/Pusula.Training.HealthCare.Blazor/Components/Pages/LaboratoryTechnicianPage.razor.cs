using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Blazor.Models.LaboratoryTechnicianPage;
using Pusula.Training.HealthCare.BloodTests;
using Pusula.Training.HealthCare.BloodTests.Reports;
using Pusula.Training.HealthCare.BloodTests.Results;
using Pusula.Training.HealthCare.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages
{
    public partial class LaboratoryTechnicianPage
    {
        private IReadOnlyList<BloodTestViewModel> BloodTestsForCompleted { get; set; }
        private IReadOnlyList<BloodTestViewModel> BloodTestsForInProgress { get; set; }
        private IReadOnlyList<BloodTestViewModel> BloodTestsForPending { get; set; }
        private List<TestInputViewModel> TestInputs { get; set; }
        private BloodTestViewModel? FillBloodTestViewModel { get; set; }
        private BloodTestResultCreateDto? BloodTestResult { get; set; }
        private string SearchValueBelow { get; set; }
        private string SearchValueUp { get; set; }
        private bool CanCreateBloodTest { get; set; }
        private bool CanEditBloodTest { get; set; }
        private bool IsVisibleCreate { get; set; }
        private bool IsVisibleEdit { get; set; }
        private bool isLoading;
        public LaboratoryTechnicianPage()
        {
            isLoading = true;
            IsVisibleCreate = false;
            IsVisibleEdit = false;
            BloodTestsForPending = [];
            BloodTestsForCompleted = [];
            BloodTestsForInProgress = [];
            TestInputs = [];
            SearchValueBelow = string.Empty;
            SearchValueUp = string.Empty;
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            isLoading = true;
            await GetAllLists();
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
            CanCreateBloodTest = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.LaboratoryTechnicians.Create);
            CanEditBloodTest = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.LaboratoryTechnicians.Edit);
        }

        public async Task GetAllLists()
        {
            BloodTestsForPending = await GetBloodTestsForPending(new GetBloodTestsInput());
            BloodTestsForInProgress = await GetBloodTestsForInProgress(new GetBloodTestsInput());
            BloodTestsForCompleted = await GetBloodTestsForCompleted(new GetBloodTestsInput());
        }

        public async Task<List<BloodTestViewModel>> GetBloodTestsByStatus(GetBloodTestsInput input, BloodTestStatus status)
        {
            input.Status = status;
            var filteredTests = await bloodTestService.GetListAsync(input);
            var bloodTests = filteredTests.Items.ToList();

            var list = await Task.WhenAll(bloodTests.Select(async bloodTest =>
            {
                var categoryIds = await bloodTestService.GetCategoryIdsAsync(bloodTest.Id);
                return new BloodTestViewModel
                {
                    BloodTestId = bloodTest.Id,
                    DoctorId = bloodTest.DoctorId,
                    PatientId = bloodTest.PatientId,
                    BloodTestDateCreated = bloodTest.DateCreated,
                    BloodTestStatus = bloodTest.Status,
                    PatientID = bloodTest.Patient.IdentityNumber,
                    PatientName = bloodTest.Patient.FirstName,
                    PatientLastName = bloodTest.Patient.LastName,
                    PatientNumber = bloodTest.Patient.PatientNumber,
                    TestCategoryIds = categoryIds
                };
            }));

            return list.ToList();
        }

        public async Task<List<BloodTestViewModel>> GetBloodTestsForPending(GetBloodTestsInput input)
        {
            return await GetBloodTestsByStatus(input, BloodTestStatus.PendingTest);
        }

        public async Task<List<BloodTestViewModel>> GetBloodTestsForInProgress(GetBloodTestsInput input)
        {
            return await GetBloodTestsByStatus(input, BloodTestStatus.InProgress);
        }

        public async Task<List<BloodTestViewModel>> GetBloodTestsForCompleted(GetBloodTestsInput input)
        {
            return await GetBloodTestsByStatus(input, BloodTestStatus.Completed);
        }

        private async Task<List<TestInputViewModel>> GetTests(List<Guid> ids)
        {
            var filteredItems = await testAppService.GetListByCategoriesAsync(ids);
            var testInputs = filteredItems.Select(test => new TestInputViewModel
            {
                TestId = test.Id,
                Name = test.Name,
                MinValue = test.MinValue,
                MaxValue = test.MaxValue
            }).ToList();
            return testInputs;
        }

        private async Task UpdatePendingStatus(BloodTestViewModel model)
        {
            await bloodTestService.UpdateAsync(new BloodTestUpdateDto
            {
                Id = model.BloodTestId,
                DoctorId = model.DoctorId,
                PatientId = model.PatientId,
                Status = BloodTestStatus.InProgress,
                DateCreated = model.BloodTestDateCreated,
                DateCompleted = model.BloodTestDateCompleted,
                TestCategoryIdList = model.TestCategoryIds,
            });
            await GetAllLists();
        }

        private async Task UpdateInProgressStatus(BloodTestViewModel model)
        {
            await bloodTestService.UpdateAsync(new BloodTestUpdateDto
            {
                Id = model.BloodTestId,
                DoctorId = model.DoctorId,
                PatientId = model.PatientId,
                Status = BloodTestStatus.Completed,
                DateCreated = model.BloodTestDateCreated,
                DateCompleted = DateTime.Now,
                TestCategoryIdList = model.TestCategoryIds,
            });
            await CloseCreateTestResultModal();
        }

        private async Task OpenCreateTestResultModal(BloodTestViewModel btViewModel)
        {
            TestInputs = await GetTests(btViewModel.TestCategoryIds);
            FillBloodTestViewModel = btViewModel;
            IsVisibleCreate = true;
        }

        private async Task OpenEditTestResultModal(BloodTestViewModel btViewModel)
        {
            var report = await bloodTestReportService.GetByBloodTestIdAsync(btViewModel.BloodTestId);

            var list = report.Results!.Select(x => new TestInputViewModel
            {
                Value = x.BloodTestResult.Value,
                TestId = x.BloodTestResult.TestId,
                MaxValue = x.BloodTestResult.Test.MaxValue,
                MinValue = x.BloodTestResult.Test.MinValue,
                Name = x.BloodTestResult.Test.Name,
                ResultId = x.BloodTestResultId
            }).ToList();
            TestInputs = list;
            IsVisibleEdit = true;
        }

        private async Task CloseCreateTestResultModal()
        {
            IsVisibleCreate = false;
            await GetAllLists();
        }

        private async Task CloseEditTestResultModal()
        {
            IsVisibleEdit = false;
            await GetAllLists();
        }

        private async Task SaveTestResults()
        {
            var bloodTestResults = TestInputs
                .Where(input => input.Value.HasValue)
                .Select(input => new BloodTestResultCreateDto
                {
                    TestId = input.TestId,
                    Value = input.Value!.Value,
                }).ToList();

            var createdResults = await Task.WhenAll(
                bloodTestResults.Select(result => bloodTestResultService.CreateAsync(result)));

            List<Guid> bloodTestResultIds = createdResults.Select(btr => btr.Id).ToList();

            await bloodTestReportService.CreateAsync(new BloodTestReportCreateDto
            {
                BloodTestId = FillBloodTestViewModel!.BloodTestId,
                BloodTestResultIds = bloodTestResultIds
            });

            await UpdateInProgressStatus(FillBloodTestViewModel);
            IsVisibleCreate = false;
            await InvokeAsync(StateHasChanged);
        }

        private async Task UpdateTestResults()
        {
            var bloodTestResults = TestInputs
                .Select(input => new BloodTestResultUpdateDto
                {
                    Id = (Guid)input.ResultId!,
                    TestId = input.TestId,
                    Value = input.Value!.Value
                }).ToList();

            foreach (var result in bloodTestResults)
            {
                await bloodTestResultService.UpdateAsync(result);
            }

            await CloseEditTestResultModal();
        }

        private async Task UpResetFilters()
        {
            SearchValueUp = string.Empty;
            await UpFilterBloodTests();
        }

        private async Task UpFilterBloodTests()
        {
            BloodTestsForPending = await GetBloodTestsForPending(new GetBloodTestsInput
            {
                FilterText = SearchValueBelow,
            });
        }

        private async Task BelowResetFilters()
        {
            SearchValueBelow = string.Empty;
            await BelowFilterBloodTests();
        }

        private async Task BelowFilterBloodTests()
        {
            BloodTestsForCompleted = await GetBloodTestsForCompleted(new GetBloodTestsInput
            {
                FilterText = SearchValueBelow,
            });
            BloodTestsForInProgress = await GetBloodTestsForInProgress(new GetBloodTestsInput
            {
                FilterText = SearchValueBelow,
            });
        }
    }
}