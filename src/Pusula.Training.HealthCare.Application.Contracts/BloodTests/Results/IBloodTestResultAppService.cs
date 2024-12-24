using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Pusula.Training.HealthCare.BloodTests.Results
{
    public interface IBloodTestResultAppService : IApplicationService
    {
        Task<PagedResultDto<BloodTestResultDto>> GetListAsync(GetBloodTestResultsInput input);
        Task<BloodTestResultDto> GetAsync(Guid id);
        Task<BloodTestResultDto> CreateAsync(BloodTestResultCreateDto input);
        Task<BloodTestResultDto> UpdateAsync(BloodTestResultUpdateDto input);
    }
}
