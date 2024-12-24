using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.BloodTests.Results
{
    public interface IBloodTestResultRepository : IRepository<BloodTestResult, Guid>
    {
        Task<BloodTestResult?> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);

        Task<List<BloodTestResult>> GetListAsync(
            string? filterText = null,
            double? value = null,
            BloodResultStatus? bloodResultStatus = null,
            Guid? testId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

        Task<long> GetCountAsync(
            string? filterText = null,
            double? value = null,
            BloodResultStatus? bloodResultStatus = null,
            Guid? testId = null,
            CancellationToken cancellationToken = default);
    }
}
