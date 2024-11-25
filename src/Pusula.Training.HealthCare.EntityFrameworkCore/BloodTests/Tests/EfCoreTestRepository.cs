using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.BloodTests.Categories;
using Pusula.Training.HealthCare.EntityFrameworkCore;
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
    public class EfCoreTestRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
        : EfCoreRepository<HealthCareDbContext, Test, Guid>(dbContextProvider), ITestRepository
    {
        public virtual async  Task<TestWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
            .Select(test => new TestWithNavigationProperties
            {
                Test = test,
                TestCategory = dbContext.Set<TestCategory>().FirstOrDefault(c => c.Id == test.TestCategoryId)!

            })
            .FirstOrDefault()!;
        }

        public virtual async Task<List<TestWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? name = null, double? minValue = null, double? maxValue = null, Guid? testCategoryId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, name, minValue, maxValue, testCategoryId);

            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }


        public virtual async Task<List<Test>> GetListAsync(string? filterText = null, string? name = null, double? minValue = null, double? maxValue = null, Guid? testCategoryId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryableAsync(), filterText, name, minValue, maxValue, testCategoryId);

            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
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

        protected virtual async Task<IQueryable<TestWithNavigationProperties>> GetQueryForNavigationPropertiesAsync() =>
           from test in (await GetDbSetAsync())
           join testCategory in (await GetDbContextAsync()).Set<TestCategory>() on test.TestCategoryId equals testCategory.Id into testCategories
           from testCategory in testCategories.DefaultIfEmpty()
           select new TestWithNavigationProperties
           {
               Test = test,
               TestCategory = testCategory
           };

        protected virtual IQueryable<TestWithNavigationProperties> ApplyFilter(
           IQueryable<TestWithNavigationProperties> query,
           string? filterText,
           string? name,
           double? minValue,
           double? maxValue,
           Guid? testCategoryId) =>
               query
                   .WhereIf(minValue.HasValue, e => e.Test.MinValue == minValue)
                   .WhereIf(maxValue.HasValue, e => e.Test.MaxValue == maxValue)
                   .WhereIf(testCategoryId.HasValue, e => e.Test.TestCategoryId == testCategoryId);
    }
}
