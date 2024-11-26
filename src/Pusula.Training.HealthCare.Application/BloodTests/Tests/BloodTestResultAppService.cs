using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    [RemoteService(IsEnabled = false)]
    [Authorize(HealthCarePermissions.BloodTestResults.Default)]
    public class BloodTestResultAppService(
        IBloodTestResultRepository bloodTestResultRepository,
        IBloodTestResultManager bloodTestResultManager,
        IBloodTestRepository bloodTestRepository,
        ITestRepository testRepository) : HealthCareAppService, IBloodTestResultAppService
    {
        public virtual async Task<PagedResultDto<BloodTestResultWithNavigationPropertiesDto>> GetListAsync(GetBloodTestResultsInput input)
        {
            var count = await bloodTestResultRepository.GetCountAsync(input.FilterText, input.Value, input.BloodResultStatus, input.BloodTestId, input.TestId);
            var items = await bloodTestResultRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Value, input.BloodResultStatus, 
                input.BloodTestId, input.TestId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<BloodTestResultWithNavigationPropertiesDto>
            {
                TotalCount = count,
                Items = ObjectMapper.Map<List<BloodTestResultWithNavigationProperties>, List<BloodTestResultWithNavigationPropertiesDto>>(items)
            };
        }
        public virtual async Task<BloodTestResultWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id) 
            => ObjectMapper.Map<BloodTestResultWithNavigationProperties, BloodTestResultWithNavigationPropertiesDto>(await bloodTestResultRepository.GetWithNavigationPropertiesAsync(id));

        public virtual async Task<BloodTestResultDto> GetAsync(Guid id) => ObjectMapper.Map<BloodTestResult, BloodTestResultDto>(await bloodTestResultRepository.GetAsync(id));

        [Authorize(HealthCarePermissions.BloodTestResults.Create)]
        public virtual async Task<BloodTestResultDto> CreateAsync(BloodTestResultCreateDto input)
        {
            if (input.BloodTestId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["BloodTest"]]);
            }
            if (input.TestId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Test"]]);
            }

            var result = await bloodTestResultManager.CreateAsync(input.BloodTestId, input.TestId, input.Value, input.BloodResultStatus);

            return ObjectMapper.Map<BloodTestResult, BloodTestResultDto>(result);
        }

        [Authorize(HealthCarePermissions.BloodTestResults.Create)]
        public virtual async Task GenerateResultsForBloodTestAsync(Guid bloodTestId)
        {
            var bloodTest = await bloodTestRepository.GetAsync(bloodTestId);
            if (bloodTest == null)
            {
                throw new UserFriendlyException(L["The specified BloodTest does not exist."]);
            }

            var relatedTests = await testRepository.GetListAsync(filterText: null, testCategoryId: bloodTest.TestCategoryId);
            if (!relatedTests.Any())
            {
                throw new UserFriendlyException(L["No Tests are associated with the specified BloodTest category."]);
            }

            foreach (var test in relatedTests)
            {
                var randomValue = GenerateRandomValue(test.MinValue, test.MaxValue);

                var status = DetermineResultStatus(randomValue, test.MinValue, test.MaxValue);

                var bloodTestResult = await bloodTestResultManager.CreateAsync(
                    bloodTestId,
                    test.Id,
                    randomValue,
                    status
                );

                await bloodTestResultRepository.InsertAsync(bloodTestResult);
            }

            bloodTest.Status = BloodTestStatus.Completed;
            await bloodTestRepository.UpdateAsync(bloodTest);
        }

        public double GenerateRandomValue(double minValue, double maxValue)
        {
            Random random = new Random();
            double value = random.NextDouble() * (maxValue - minValue) + minValue;
            return Math.Floor(value * 1000) / 1000;  
        }

        private BloodResultStatus DetermineResultStatus(double value, double minValue, double maxValue)
        {
            if (value < minValue)
                return BloodResultStatus.Low;
            if (value > maxValue)
                return BloodResultStatus.High;

            return BloodResultStatus.Normal;
        }

    }
}
