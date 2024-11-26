using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    [RemoteService(IsEnabled = false)]
    public class TestAppService(
        ITestRepository testRepository,
        ITestManager testManager) : HealthCareAppService, ITestAppService
    {
        public virtual async Task<PagedResultDto<TestWithNavigationPropertiesDto>> GetListAsync(GetTestsInput input)
        {
            var count = await testRepository.GetCountAsync(input.FilterText, input.Name, input.MinValue, input.MaxValue, input.TestCategoryId);
            var items = await testRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Name, input.MinValue, input.MaxValue, input.TestCategoryId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<TestWithNavigationPropertiesDto>
            {
                TotalCount = count,
                Items = ObjectMapper.Map<List<TestWithNavigationProperties>, List<TestWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<TestWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
            => ObjectMapper.Map<TestWithNavigationProperties, TestWithNavigationPropertiesDto>(await testRepository.GetWithNavigationPropertiesAsync(id));

        public virtual async Task<TestDto> GetAsync(Guid id) => ObjectMapper.Map<Test, TestDto>(await testRepository.GetAsync(id));

        public virtual async Task<TestDto> CreateAsync(TestCreateDto input)
        {
            if (input.TestCategoryId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["TestCategory"]]);
            }

            var result = await testManager.CreateAsync(input.TestCategoryId, input.Name, input.MinValue, input.MaxValue);

            return ObjectMapper.Map<Test, TestDto>(result);
        }
    }
}
