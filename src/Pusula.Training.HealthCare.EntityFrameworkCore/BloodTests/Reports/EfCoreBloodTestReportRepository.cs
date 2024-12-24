using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.BloodTests.Reports
{
    public class EfCoreBloodTestReportRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
        : EfCoreRepository<HealthCareDbContext, BloodTestReport, Guid>(dbContextProvider), IBloodTestReportRepository
    {
        public virtual async Task<BloodTestReport?> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public virtual async Task<BloodTestReport> GetByBloodTestIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var report = ApplyFilter(await GetQueryForNavigationPropertiesAsync(), bloodTestId: id).FirstOrDefault();
            return report!;
        }

        public virtual async Task<List<BloodTestReport>> GetListByPatientNumberAsync(int patientNumber, CancellationToken cancellationToken = default)
        {
            var query =await GetQueryForNavigationPropertiesAsync();
            query = query.OrderByDescending(x => x.BloodTest.DateCreated);
            var report = query.Where(x => x.BloodTest.Patient.PatientNumber == patientNumber).ToList();
            return report;
        }

        public virtual async Task<List<BloodTestReport>> GetListAsync(string? filterText = null, string? sorting = null, Guid? bloodTestId = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryForNavigationPropertiesAsync(), filterText, bloodTestId);
            query = query.OrderByDescending(x => x.BloodTest.DateCreated);    
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<List<BloodTestReportResult>> GetFilteredResultsByPatientAndTestAsync(Guid patientId, Guid testId, CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            return await query
                .Where(report => report.BloodTest.PatientId == patientId)
                .SelectMany(report => report.Results
                    .Where(result => result.BloodTestResult.TestId == testId))
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(string? filterText = null, Guid? bloodTestId = null, CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryForNavigationPropertiesAsync(), filterText, bloodTestId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<BloodTestReport> ApplyFilter(
            IQueryable<BloodTestReport> query,
            string? filterText = null,
            Guid? bloodTestId = null) =>
                query
                    .WhereIf(bloodTestId.HasValue, e => e.BloodTestId == bloodTestId);


        protected virtual async Task<IQueryable<BloodTestReport>> GetQueryForNavigationPropertiesAsync()
            => (await GetQueryableAsync())
                .Include(b => b.BloodTest)
                    .ThenInclude(p => p.Patient)
                .Include(b => b.BloodTest)
                    .ThenInclude(d => d.Doctor)
                .Include(btr => btr.Results)
                    .ThenInclude(r => r.BloodTestResult)
                    .ThenInclude(c => c.Test)
                    .ThenInclude(v => v.TestCategory);
    }
}
