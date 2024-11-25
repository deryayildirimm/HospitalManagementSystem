using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.BloodTests.Categories;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using Pusula.Training.HealthCare.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.BloodTests
{
    public class EfCoreBloodTestRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
        : EfCoreRepository<HealthCareDbContext, BloodTest, Guid>(dbContextProvider), IBloodTestRepository
    {
        public async Task<BloodTestWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(bloodTest => new BloodTestWithNavigationProperties
                {
                    BloodTest = bloodTest,
                    Doctor = dbContext.Set<Doctor>().FirstOrDefault(c => c.Id == bloodTest.DoctorId)!,
                    Patient = dbContext.Set<Patient>().FirstOrDefault(c => c.Id == bloodTest.PatientId)!,
                    TestCategory = dbContext.Set<TestCategory>().FirstOrDefault(c => c.Id == bloodTest.TestCategoryId)!
                }).FirstOrDefault()!;
        }

        public async Task<List<BloodTestWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            BloodTestStatus? status = null,
            DateTime? dateCreatedMin = null,
            DateTime? dateCreatedMax = null,
            DateTime? dateCompletedMin = null,
            DateTime? dateCompletedMax = null,
            Guid? doctorId = null,
            Guid? patientId = null,
            Guid? testCategoryId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryForNavigationPropertiesAsync(), filterText, status, dateCreatedMin, dateCreatedMax, dateCompletedMin, dateCompletedMax, doctorId, patientId, testCategoryId);
            //query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? BloodTestConst.GetBloodTestDefaultSorting(false) : sorting);

            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<List<BloodTest>> GetListAsync(
            string? filterText = null,
            BloodTestStatus? status = null,
            DateTime? dateCreatedMin = null,
            DateTime? dateCreatedMax = null,
            DateTime? dateCompletedMin = null,
            DateTime? dateCompletedMax = null,
            Guid? doctorId = null,
            Guid? patientId = null,
            Guid? testCategoryId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryableAsync(), filterText, status, dateCreatedMin, dateCreatedMax, dateCompletedMin, dateCompletedMax, doctorId, patientId, testCategoryId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? BloodTestConst.GetBloodTestDefaultSorting(false) : sorting);

            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
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
            Guid? testCategoryId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, status, dateCreatedMin, dateCreatedMax, dateCompletedMin, dateCompletedMax, doctorId, patientId, testCategoryId);

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
            Guid? patientId = null,
            Guid? testCategoryId = null) =>
                query
                    .WhereIf(status.HasValue, e => e.Status == status)  
                    .WhereIf(dateCompletedMin.HasValue, e => e.DateCreated >= dateCreatedMin!.Value)
                    .WhereIf(dateCompletedMax.HasValue, e => e.DateCreated <= dateCreatedMax!.Value)
                    .WhereIf(dateCompletedMin.HasValue, e => e.DateCompleted >= dateCompletedMin!.Value)
                    .WhereIf(dateCompletedMax.HasValue, e => e.DateCompleted <= dateCompletedMax!.Value)
                    .WhereIf(doctorId.HasValue, e => e.DoctorId == doctorId)
                    .WhereIf(patientId.HasValue, e => e.PatientId == patientId)
                    .WhereIf(testCategoryId.HasValue, e => e.TestCategoryId == testCategoryId);

        protected virtual async Task<IQueryable<BloodTestWithNavigationProperties>> GetQueryForNavigationPropertiesAsync() =>
            from bloodTest in (await GetDbSetAsync())
            join doctor in (await GetDbContextAsync()).Set<Doctor>() on bloodTest.DoctorId equals doctor.Id into doctors
            from doctor in doctors.DefaultIfEmpty()
            join patient in (await GetDbContextAsync()).Set<Patient>() on bloodTest.PatientId equals patient.Id into patients
            from patient in patients.DefaultIfEmpty()
            join testCategory in (await GetDbContextAsync()).Set<TestCategory>() on bloodTest.TestCategoryId equals testCategory.Id into testCategories
            from testCategory in testCategories.DefaultIfEmpty()
            select new BloodTestWithNavigationProperties
            {
                BloodTest = bloodTest,
                Doctor = doctor,
                Patient = patient,
                TestCategory = testCategory
            };

        protected virtual IQueryable<BloodTestWithNavigationProperties> ApplyFilter(
            IQueryable<BloodTestWithNavigationProperties> query,
            string? filterText = null,
            BloodTestStatus? status = null,
            DateTime? dateCreatedMin = null,
            DateTime? dateCreatedMax = null,
            DateTime? dateCompletedMin = null,
            DateTime? dateCompletedMax = null,
            Guid? doctorId = null,
            Guid? patientId = null,
            Guid? testCategoryId = null) =>
                query
                    .WhereIf(status.HasValue, e => e.BloodTest.Status == status)
                    .WhereIf(dateCompletedMin.HasValue, e => e.BloodTest.DateCreated >= dateCreatedMin!.Value)
                    .WhereIf(dateCompletedMax.HasValue, e => e.BloodTest.DateCreated <= dateCreatedMax!.Value)
                    .WhereIf(dateCompletedMin.HasValue, e => e.BloodTest.DateCompleted >= dateCompletedMin!.Value)
                    .WhereIf(dateCompletedMax.HasValue, e => e.BloodTest.DateCompleted <= dateCompletedMax!.Value)
                    .WhereIf(doctorId.HasValue, e => e.BloodTest.DoctorId == doctorId)
                    .WhereIf(patientId.HasValue, e => e.BloodTest.PatientId == patientId)
                    .WhereIf(testCategoryId.HasValue, e => e.BloodTest.TestCategoryId == testCategoryId);
    }
}
