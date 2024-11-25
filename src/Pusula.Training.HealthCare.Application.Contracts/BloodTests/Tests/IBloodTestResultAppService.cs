using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public interface IBloodTestResultAppService : IApplicationService
    {
        Task<PagedResultDto<BloodTestResultWithNavigationPropertiesDto>> GetListAsync(GetBloodTestResultsInput input);

        Task<BloodTestResultWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<BloodTestResultDto> GetAsync(Guid id);

        Task<BloodTestResultDto> CreateAsync(BloodTestResultCreateDto input);
        Task GenerateResultsForBloodTestAsync(Guid bloodTestId);

    }
}
