using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public interface ITestAppService  : IApplicationService
    {
        Task<PagedResultDto<TestWithNavigationPropertiesDto>> GetListAsync(GetTestsInput input);

        Task<TestWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<TestDto> GetAsync(Guid id);

        Task<TestDto> CreateAsync(TestCreateDto input);
    }
}
