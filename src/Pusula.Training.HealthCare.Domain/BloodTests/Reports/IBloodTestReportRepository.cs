using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.BloodTests.Reports
{
    public interface IBloodTestReportRepository : IRepository<BloodTestReport, Guid>
    {
        Task<BloodTestReport?> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);

        Task<List<BloodTestReport>> GetListAsync(
            string? filterText = null,
            string? sorting = null,
            Guid? bloodTestId = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
            );

        Task<long> GetCountAsync(
            string? filterText = null,
            Guid? bloodTestId = null,
            CancellationToken cancellationToken = default
            );

        Task<BloodTestReport> GetByBloodTestIdAsync(
            Guid id,
            CancellationToken cancellationToken = default
            );

        Task<List<BloodTestReport>> GetListByPatientNumberAsync(
            int patientNumber, 
            CancellationToken cancellationToken = default 
            );

        Task<List<BloodTestReportResult>> GetFilteredResultsByPatientAndTestAsync(
            Guid patientId,
            Guid testId,
            CancellationToken cancellationToken = default);
    }
}
