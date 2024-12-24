using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public interface ITestAppService  : IApplicationService
    {
        Task<PagedResultDto<TestDto>> GetListAsync(GetTestsInput input);

        Task<TestDto> GetAsync(Guid id);

        Task<TestDto> CreateAsync(TestCreateDto input);
        Task<TestDto> UpdateAsync(TestUpdateDto input);

        Task<List<TestDto>> GetListByCategoriesAsync(List<Guid> categoryIds);

    }
}
