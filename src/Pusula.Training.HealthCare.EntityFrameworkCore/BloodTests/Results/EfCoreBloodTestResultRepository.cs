using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.BloodTests.Results
{
    public class EfCoreBloodTestResultRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
        : EfCoreRepository<HealthCareDbContext, BloodTestResult, Guid>(dbContextProvider), IBloodTestResultRepository
    {        
        public virtual async Task<BloodTestResult?> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public virtual async Task<List<BloodTestResult>> GetListAsync(
            string? filterText = null,
            double? value = null,
            BloodResultStatus? bloodResultStatus = null,
            Guid? testId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryForNavigationPropertiesAsync(), filterText, value, bloodResultStatus, testId);

            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null, 
            double? value = null, 
            BloodResultStatus? bloodResultStatus = null, 
            Guid? testId = null, 
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryForNavigationPropertiesAsync(), filterText, value, bloodResultStatus, testId);

            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<BloodTestResult> ApplyFilter(
            IQueryable<BloodTestResult> query,
            string? filterText = null,
            double? value = null,
            BloodResultStatus? bloodResultStatus = null,
            Guid? testId = null) =>
                query
                    .WhereIf(value.HasValue, e => e.Value == value)
                    .WhereIf(bloodResultStatus.HasValue, e => e.BloodResultStatus == bloodResultStatus)
                    .WhereIf(testId.HasValue, e => e.TestId == testId);

        protected virtual async Task<IQueryable<BloodTestResult>> GetQueryForNavigationPropertiesAsync()
        =>
            (await GetQueryableAsync())
            .Include(x => x.Test)
                .ThenInclude(x => x.TestCategory);
    }
}
