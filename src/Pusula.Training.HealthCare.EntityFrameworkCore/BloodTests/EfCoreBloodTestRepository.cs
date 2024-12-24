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

namespace Pusula.Training.HealthCare.BloodTests
{
    public class EfCoreBloodTestRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
        : EfCoreRepository<HealthCareDbContext, BloodTest, Guid>(dbContextProvider), IBloodTestRepository
    {
        public virtual async Task<BloodTest?> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var query = (await GetQueryForNavigationPropertiesAsync());

            return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public virtual async Task<List<BloodTest>> GetListAsync(
            string? filterText = null,
            BloodTestStatus? status = null,
            DateTime? dateCreatedMin = null,
            DateTime? dateCreatedMax = null,
            DateTime? dateCompletedMin = null,
            DateTime? dateCompletedMax = null,
            Guid? doctorId = null,
            Guid? patientId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryForNavigationPropertiesAsync(), filterText, status, dateCreatedMin, dateCreatedMax, dateCompletedMin, dateCompletedMax, doctorId, patientId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? BloodTestConst.GetBloodTestDefaultSorting(false) : sorting);

            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<List<Guid>> GetCategoryIdsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var bloodTest = await GetWithNavigationPropertiesAsync(id, cancellationToken);

            return bloodTest?.BloodTestCategories
                .Select(c => c.TestCategoryId)
                .ToList() ?? new List<Guid>();
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            BloodTestStatus? status = null,
            DateTime? dateCreatedMin = null,
            DateTime? dateCreatedMax = null,
            DateTime? dateCompletedMin = null,
            DateTime? dateCompletedMax = null,
            Guid? doctorId = null,
            Guid? patientId = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryForNavigationPropertiesAsync(), filterText, status, dateCreatedMin, dateCreatedMax, 
                    dateCompletedMin, dateCompletedMax, doctorId, patientId);

            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<BloodTest> ApplyFilter(
            IQueryable<BloodTest> query,
            string? filterText = null,
            BloodTestStatus? status = null,
            DateTime? dateCreatedMin = null,
            DateTime? dateCreatedMax = null,
            DateTime? dateCompletedMin = null,
            DateTime? dateCompletedMax = null,
            Guid? doctorId = null,
            Guid? patientId = null) =>
                query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => EF.Functions.ILike(e.Patient.FirstName, $"%{filterText}%")   
                                                                        || EF.Functions.ILike(e.Patient.LastName, $"%{filterText}%"))
                    .WhereIf(status.HasValue, e => e.Status == status)
                    .WhereIf(dateCreatedMin.HasValue, e => e.DateCreated >= dateCreatedMin!.Value)
                    .WhereIf(dateCreatedMax.HasValue, e => e.DateCreated <= dateCreatedMax!.Value)
                    .WhereIf(dateCompletedMin.HasValue, e => e.DateCompleted >= dateCompletedMin!.Value)
                    .WhereIf(dateCompletedMax.HasValue, e => e.DateCompleted <= dateCompletedMax!.Value)
                    .WhereIf(doctorId.HasValue, e => e.DoctorId == doctorId)
                    .WhereIf(patientId.HasValue, e => e.PatientId == patientId);


        protected virtual async Task<IQueryable<BloodTest>> GetQueryForNavigationPropertiesAsync()
        =>
            (await GetQueryableAsync())
            .Include(x => x.Doctor)
                .ThenInclude(d => d.Department)
            .Include(x => x.Doctor)
                .ThenInclude(d => d.Title)
            .Include(x => x.Patient)
            .Include(x => x.BloodTestCategories)
                .ThenInclude(btc => btc.TestCategory);

    }
}
