using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.BloodTests;
using Pusula.Training.HealthCare.BloodTests.Category;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Titles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages
{
    public partial class BloodTests
    {
        private bool CanEditBloodTest { get; set; }
        private GetBloodTestsInput Filter { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private int TotalCount { get; set; }
        private string CurrentSorting { get; set; } = string.Empty;

        private Dictionary<GroupedBloodTestDto, bool> expandedCards = new();
        private List<GroupedBloodTestDto> groupedBloodTests { get; set; } = null!;
        private bool isLoading = true;

        public BloodTests()
        {
            Filter = new GetBloodTestsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
        }
        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            isLoading = true;
            groupedBloodTests = await GetGroupedBloodTestsAsync(new GetBloodTestsInput());
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
        private async Task<List<GroupedBloodTestDto>> GetGroupedBloodTestsAsync(GetBloodTestsInput input)
        {
            input.Status = BloodTestStatus.Requested;
            var bloodTestListResult = await bloodTestAppService.GetListAsync(input);

            var groupedBloodTests = bloodTestListResult.Items
                .GroupBy(bt => new
                {
                    PatientId = bt.Patient.Id,       
                    TestDate = bt.BloodTest.DateCreated, 
                    DoctorId = bt.Doctor.Id
                })
                .OrderByDescending(group => group.Key.TestDate)
                .Select(async group =>
                {
                    var patient = await patientAppService.GetAsync(group.Key.PatientId);
                    var doctor = await doctorAppService.GetAsync(group.Key.DoctorId);

                    DepartmentDto? department = await departmentAppService.GetAsync(doctor.DepartmentId);
                    TitleDto? title = await titleAppService.GetAsync(doctor.TitleId);
                    var status = group.FirstOrDefault()?.BloodTest.Status.ToString();
                    var categoriesWithDetails = group
                        .Select(g => new TestCategoryDto
                        {
                            Name = g.TestCategory.Name,
                            Description = g.TestCategory.Description,
                            Url = g.TestCategory.Url,
                            Price = g.TestCategory.Price
                        }).Distinct().ToList();

                    return new GroupedBloodTestDto
                    {
                        Patient = patient,
                        Doctor = doctor,
                        DateRequested = group.Key.TestDate,
                        SelectedCategories = categoriesWithDetails, 
                        TestIds = group.Select(g => g.BloodTest.Id).ToList(),
                        Department = department!,
                        Title = title!,
                        Status = status!
                    };
                });

            var result = await Task.WhenAll(groupedBloodTests);
            return result.ToList();
        }

        private void ToggleExpand(GroupedBloodTestDto card)
        {
            if (expandedCards.ContainsKey(card))
            {
                expandedCards[card] = !expandedCards[card];
            }
            else
            {
                expandedCards[card] = true;
            }
        }

        private async Task UpdateTestsStatusAndGenerateResultsAsync(GroupedBloodTestDto card)
        {
            
            var updateDtos = card.TestIds.Select(testId => new BloodTestUpdateDto
            {
                Id = testId,
                Status = BloodTestStatus.InProgress, 
                DateCreated = card.DateRequested,
                DateCompleted = DateTime.Now,
                DoctorId = card.Doctor.Id,
                PatientId = card.Patient.Id,
                TestCategoryId = card.SelectedCategories.First().Id 
            }).ToList();

            await bloodTestAppService.BulkUpdateStatusAsync(updateDtos);

            foreach (var testId in card.TestIds)
            {
                await bloodTestResultAppService.GenerateResultsForBloodTestAsync(testId);
            }

            groupedBloodTests = await GetGroupedBloodTestsAsync(new GetBloodTestsInput());
        }

    }
}