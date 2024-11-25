using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public interface ITestRepository : IRepository<Test, Guid>
    {
        Task<TestWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<TestWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? name = null,
            double? minValue = null,
            double? maxValue = null,
            Guid? testCategoryId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

        Task<List<Test>> GetListAsync(
            string? filterText = null,
            string? name = null,
            double? minValue = null,
            double? maxValue = null,
            Guid? testCategoryId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

        Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            double? minValue = null,
            double? maxValue = null,
            Guid? testCategoryId = null,
            CancellationToken cancellationToken = default);
    }
}
