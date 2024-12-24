using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public class EfCoreTestRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
        : EfCoreRepository<HealthCareDbContext, Test, Guid>(dbContextProvider), ITestRepository
    {
        
        public virtual async Task<Test?> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var query = (await GetQueryableAsync())
                .Include(x => x.TestCategory);

            return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public virtual async Task<List<Test>> GetListAsync(string? filterText = null, string? name = null, double? minValue = null, double? maxValue = null, Guid? testCategoryId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryableAsync(), filterText, name, minValue, maxValue, testCategoryId);

            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<List<Test>> GetListByCategoriesAsync(List<Guid> categoryIds, CancellationToken cancellationToken = default)
        {
            var query = await GetQueryableAsync();
            var list = query.Where(t => categoryIds.Contains(t.TestCategoryId)).ToList();
            return list;
        }

        public virtual async Task<long> GetCountAsync(string? filterText = null, string? name = null, double? minValue = null, double? maxValue = null, Guid? testCategoryId = null, CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryableAsync(), filterText, name, minValue, maxValue, testCategoryId);

            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Test> ApplyFilter(
            IQueryable<Test> query,
            string? filterText,
            string? name,
            double? minValue,
            double? maxValue,
            Guid? testCategoryId) =>
                query
                    .WhereIf(minValue.HasValue, e => e.MinValue == minValue)
                    .WhereIf(maxValue.HasValue, e => e.MaxValue == maxValue)
                    .WhereIf(testCategoryId.HasValue, e => e.TestCategoryId == testCategoryId);
    }
}
