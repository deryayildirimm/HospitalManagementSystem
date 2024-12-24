using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.GlobalExceptions;
using Pusula.Training.HealthCare.Permissions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    [RemoteService(IsEnabled = false)]
    [Authorize(HealthCarePermissions.BloodTests.Create)]
    public class TestAppService(
        ITestRepository testRepository,
        ITestManager testManager) : HealthCareAppService, ITestAppService
    {
        public virtual async Task<PagedResultDto<TestDto>> GetListAsync(GetTestsInput input)
        {
            var count = await testRepository.GetCountAsync(input.FilterText, input.Name, input.MinValue, input.MaxValue, input.TestCategoryId);
            var items = await testRepository.GetListAsync(input.FilterText, input.Name, input.MinValue, input.MaxValue, input.TestCategoryId, input.Sorting, input.MaxResultCount, input.SkipCount);
            return new PagedResultDto<TestDto>
            {
                TotalCount = count,
               Items = ObjectMapper.Map<List<Test>, List<TestDto>>(items)
            };
        }

        public virtual async Task<TestDto> GetAsync(Guid id) => ObjectMapper.Map<Test, TestDto>(await testRepository.GetAsync(id));

        [Authorize(HealthCarePermissions.BloodTests.Create)]
        public virtual async Task<TestDto> CreateAsync(TestCreateDto input)
        {
            HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.TestCategoryInformationsRequired, input.TestCategoryId == default);

            var result = await testManager.CreateAsync(input.TestCategoryId, input.Name, input.MinValue, input.MaxValue);

            return ObjectMapper.Map<Test, TestDto>(result);
        }

        [Authorize(HealthCarePermissions.BloodTests.Edit)]
        public virtual async Task<TestDto> UpdateAsync(TestUpdateDto input)
        {
            HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.TestCategoryInformationsRequired, input.TestCategoryId == default);

            var test = await testManager.UpdateAsync(input.Id, input.TestCategoryId, input.Name, input.MinValue, input.MaxValue);

            return ObjectMapper.Map<Test, TestDto>(test);
        }

        public virtual async Task<List<TestDto>> GetListByCategoriesAsync(List<Guid> categoryIds) 
            => ObjectMapper.Map<List<Test>,List<TestDto>>(await testRepository.GetListByCategoriesAsync(categoryIds));
    }
}
