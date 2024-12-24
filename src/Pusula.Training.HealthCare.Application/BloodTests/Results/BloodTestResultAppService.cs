using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.GlobalExceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using static Pusula.Training.HealthCare.Permissions.HealthCarePermissions;

namespace Pusula.Training.HealthCare.BloodTests.Results
{
    [RemoteService(IsEnabled = false)]
    [Authorize(BloodTestResults.Default)]
    public class BloodTestResultAppService(
        IBloodTestResultRepository bloodTestResultRepository,
        IBloodTestResultManager bloodTestResultManager) : HealthCareAppService, IBloodTestResultAppService
    {
        public virtual async Task<PagedResultDto<BloodTestResultDto>> GetListAsync(GetBloodTestResultsInput input)
        {
            var count = await bloodTestResultRepository.GetCountAsync(input.FilterText, input.Value, input.BloodResultStatus, input.TestId);
            var items = await bloodTestResultRepository.GetListAsync(input.FilterText, input.Value, input.BloodResultStatus,
                input.TestId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<BloodTestResultDto>
            {
                Items = ObjectMapper.Map<List<BloodTestResult>, List<BloodTestResultDto>>(items),
                TotalCount = count
            };
        }

        public virtual async Task<BloodTestResultDto> GetAsync(Guid id) => ObjectMapper.Map<BloodTestResult, BloodTestResultDto>(await bloodTestResultRepository.GetAsync(id));

        [Authorize(BloodTestResults.Create)]
        public virtual async Task<BloodTestResultDto> CreateAsync(BloodTestResultCreateDto input)
        {
            HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.TestInformationsRequired, input.TestId == default);

            var result = await bloodTestResultManager.CreateAsync(input.TestId, input.Value);

            return ObjectMapper.Map<BloodTestResult, BloodTestResultDto>(result);
        }

        [Authorize(BloodTestResults.Edit)]
        public virtual async Task<BloodTestResultDto> UpdateAsync(BloodTestResultUpdateDto input)
        {
            HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.TestInformationsRequired, input.TestId == default);

            var resut = await bloodTestResultManager.UpdateAsync(input.Id, input.TestId, input.Value);

            return ObjectMapper.Map<BloodTestResult, BloodTestResultDto>(resut);
        }
    }
}
