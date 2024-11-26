using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public interface IBloodTestResultRepository : IRepository<BloodTestResult, Guid>
    {
        Task<BloodTestResultWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);

        Task<List<BloodTestResultWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            double? value = null,
            BloodResultStatus? bloodResultStatus = null,
            Guid? bloodTestId = null,
            Guid? testId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

        Task<List<BloodTestResult>> GetListAsync(
            string? filterText = null,
            double? value = null,
            BloodResultStatus? bloodResultStatus = null,
            Guid? bloodTestId = null,
            Guid? testId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

        Task<long> GetCountAsync(
            string? filterText = null,
            double? value = null,
            BloodResultStatus? bloodResultStatus = null,
            Guid? bloodTestId = null,
            Guid? testId = null,
            CancellationToken cancellationToken = default);
    }
}
