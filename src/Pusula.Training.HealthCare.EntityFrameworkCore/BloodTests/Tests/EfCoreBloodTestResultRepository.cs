using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using Pusula.Training.HealthCare.Titles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using static Pusula.Training.HealthCare.Permissions.HealthCarePermissions;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public class EfCoreBloodTestResultRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
        : EfCoreRepository<HealthCareDbContext, BloodTestResult, Guid>(dbContextProvider), IBloodTestResultRepository
    {
        public virtual async Task<BloodTestResultWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
            .Select(result => new BloodTestResultWithNavigationProperties
            {
                BloodTestResult = result,
                BloodTest = dbContext.Set<BloodTest>().FirstOrDefault(c => c.Id == result.BloodTestId)!,
                Test = dbContext.Set<Test>().FirstOrDefault(c => c.Id == result.TestId)!
            })
            .FirstOrDefault()!;
        }
        public virtual async Task<List<BloodTestResultWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null, 
            double? value = null, 
            BloodResultStatus? bloodResultStatus = null, 
            Guid? bloodTestId = null, 
            Guid? testId = null, 
            string? sorting = null, 
            int maxResultCount = int.MaxValue, 
            int skipCount = 0, 
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, value, bloodResultStatus, bloodTestId, testId);

            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }
        public virtual async Task<List<BloodTestResult>> GetListAsync(
            string? filterText = null, 
            double? value = null, 
            BloodResultStatus? bloodResultStatus = null, 
            Guid? bloodTestId = null, 
            Guid? testId = null, 
            string? sorting = null, 
            int maxResultCount = int.MaxValue, 
            int skipCount = 0, 
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryableAsync(), filterText, value, bloodResultStatus, bloodTestId, testId);

            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(string? filterText = null, double? value = null, BloodResultStatus? bloodResultStatus = null, Guid? bloodTestId = null, Guid? testId = null, CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryableAsync(), filterText, value, bloodResultStatus, bloodTestId, testId);

            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<BloodTestResult> ApplyFilter(
            IQueryable<BloodTestResult> query,
            string? filterText = null,
            double? value = null,
            BloodResultStatus? bloodResultStatus = null,
            Guid? bloodTestId = null,
            Guid? testId = null) =>
                query
                    .WhereIf(value.HasValue, e => e.Value == value)
                    .WhereIf(bloodResultStatus.HasValue, e => e.BloodResultStatus == bloodResultStatus)
                    .WhereIf(bloodTestId.HasValue, e => e.BloodTestId == bloodTestId)
                    .WhereIf(testId.HasValue, e => e.TestId == testId);

        protected virtual async Task<IQueryable<BloodTestResultWithNavigationProperties>> GetQueryForNavigationPropertiesAsync() =>
            from result in (await GetDbSetAsync())
            join bloodTest in (await GetDbContextAsync()).Set<BloodTest>() on result.BloodTestId equals bloodTest.Id into bloodTests
            from bloodTest in bloodTests.DefaultIfEmpty()
            join test in (await GetDbContextAsync()).Set<Test>() on result.TestId equals test.Id into tests
            from test in tests.DefaultIfEmpty()
            select new BloodTestResultWithNavigationProperties
            {
                BloodTestResult = result,
                BloodTest = bloodTest,
                Test = test
            };

        protected virtual IQueryable<BloodTestResultWithNavigationProperties> ApplyFilter(
            IQueryable<BloodTestResultWithNavigationProperties> query,
            string? filterText = null,
            double? value = null,
            BloodResultStatus? bloodResultStatus = null,
            Guid? bloodTestId = null,
            Guid? testId = null) =>
                query
                    .WhereIf(value.HasValue, e => e.BloodTestResult.Value == value)
                    .WhereIf(bloodResultStatus.HasValue, e => e.BloodTestResult.BloodResultStatus == bloodResultStatus)
                    .WhereIf(bloodTestId.HasValue, e => e.BloodTestResult.BloodTestId == bloodTestId)
                    .WhereIf(testId.HasValue, e => e.BloodTestResult.TestId == testId);
    }
}
