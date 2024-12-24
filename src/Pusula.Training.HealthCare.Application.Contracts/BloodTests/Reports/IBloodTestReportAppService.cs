using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Pusula.Training.HealthCare.BloodTests.Reports
{
    public interface IBloodTestReportAppService : IApplicationService
    {
        Task<PagedResultDto<BloodTestReportDto>> GetListAsync(GetBloodTestReportsInput input);
        Task<BloodTestReportDto> GetAsync(Guid id);
        Task<BloodTestReportDto> CreateAsync(BloodTestReportCreateDto input);
        Task<BloodTestReportDto> UpdateAsync(BloodTestReportUpdateDto input);
        Task<BloodTestReportDto> GetByBloodTestIdAsync(Guid id);
        Task<List<BloodTestReportDto>> GetListByPatientNumberAsync(int patientNumber);
        Task<List<BloodTestReportResultDto>> GetFilteredResultsByPatientAndTestAsync(Guid patientId, Guid testId, CancellationToken cancellationToken = default);
    }
}
