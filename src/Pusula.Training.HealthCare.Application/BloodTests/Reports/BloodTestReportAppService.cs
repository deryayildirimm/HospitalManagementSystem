using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.GlobalExceptions;
using Pusula.Training.HealthCare.Permissions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.BloodTests.Reports
{
    [RemoteService(IsEnabled = false)]
    [Authorize(HealthCarePermissions.LaboratoryTechnicians.Default)]
    public class BloodTestReportAppService(
        IBloodTestReportRepository bloodTestReportRepository,
        IBloodTestReportManager bloodTestReportManager) : HealthCareAppService, IBloodTestReportAppService
    {
        public virtual async Task<PagedResultDto<BloodTestReportDto>> GetListAsync(GetBloodTestReportsInput input)
        {
            var totalCount = await bloodTestReportRepository.GetCountAsync(input.FilterText, input.BloodTestId);
            var items = await bloodTestReportRepository.GetListAsync(input.FilterText, input.Sorting, input.BloodTestId, input.MaxResultCount, input.SkipCount);
            return new PagedResultDto<BloodTestReportDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<BloodTestReport>, List<BloodTestReportDto>>(items)
            };
        }

        public virtual async Task<BloodTestReportDto> GetAsync(Guid id)
        {
            var result = await bloodTestReportRepository.GetWithNavigationPropertiesAsync(id);
            return ObjectMapper.Map<BloodTestReport, BloodTestReportDto>(result!);
        }

        [Authorize(HealthCarePermissions.LaboratoryTechnicians.Create)]
        public virtual async Task<BloodTestReportDto> CreateAsync(BloodTestReportCreateDto input)
        {
            HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.BloodTestInformationsRequired, input.BloodTestId == default);

            var bloodTestReport = await bloodTestReportManager.CreateAsync(input.BloodTestId, input.BloodTestResultIds);

            return ObjectMapper.Map<BloodTestReport, BloodTestReportDto>(bloodTestReport);
        }

        [Authorize(HealthCarePermissions.LaboratoryTechnicians.Edit)]
        public virtual async Task<BloodTestReportDto> UpdateAsync(BloodTestReportUpdateDto input)
        {
            HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.BloodTestInformationsRequired, input.BloodTestId == default);

            var bloodTestReport = await bloodTestReportManager.UpdateAsync(input.Id, input.BloodTestId, input.BloodTestResultIds);

            return ObjectMapper.Map<BloodTestReport, BloodTestReportDto>(bloodTestReport);
        }

        public virtual async Task<BloodTestReportDto> GetByBloodTestIdAsync(Guid id)
            => ObjectMapper.Map<BloodTestReport, BloodTestReportDto>(await bloodTestReportRepository.GetByBloodTestIdAsync(id));
        

        public virtual async Task<List<BloodTestReportDto>> GetListByPatientNumberAsync(int patientNumber)
            => ObjectMapper.Map<List<BloodTestReport>, List<BloodTestReportDto>>(await bloodTestReportRepository.GetListByPatientNumberAsync(patientNumber));

        public virtual async Task<List<BloodTestReportResultDto>> GetFilteredResultsByPatientAndTestAsync(Guid patientId, Guid testId, CancellationToken cancellationToken = default)
            => ObjectMapper.Map<List<BloodTestReportResult>, List<BloodTestReportResultDto>>
            (await bloodTestReportRepository.GetFilteredResultsByPatientAndTestAsync(patientId, testId, cancellationToken));
    }
}
