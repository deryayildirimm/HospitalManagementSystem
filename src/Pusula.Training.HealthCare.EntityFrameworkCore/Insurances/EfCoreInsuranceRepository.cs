using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.Insurances
{
    public class EfCoreInsuranceRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
        : EfCoreRepository<HealthCareDbContext, Insurance, Guid>(dbContextProvider), IInsuranceRepository
    {
        public virtual async Task<List<Insurance>> GetListAsync(
            string? filterText = null,
            string? policyNumber = null,
            decimal? premiumAmount = null,
            decimal? coverageAmount = null,
            DateTime? startDateMin = null,
            DateTime? startDateMax = null,
            DateTime? endDateMin = null,
            DateTime? endDateMax = null,
            EnumInsuranceCompanyName? insuranceCompanyName = null,
            string? description = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryableAsync(), filterText, policyNumber, premiumAmount, coverageAmount, startDateMin, startDateMax,
                endDateMin, endDateMax, insuranceCompanyName, description);
            //query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? InsuranceConst.GetDefaultSorting(true) : sorting);
            return query.PageBy(skipCount, maxResultCount).ToList();
        }

        public async virtual Task DeleteAllAsync(
            string? filterText = null,
            string? policyNumber = null,
            decimal? premiumAmount = null,
            decimal? coverageAmount = null,
            DateTime? startDateMin = null,
            DateTime? startDateMax = null,
            DateTime? endDateMin = null,
            DateTime? endDateMax = null,
            EnumInsuranceCompanyName? insuranceCompanyName = null,
            string? description = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryableAsync(), filterText, policyNumber, premiumAmount, coverageAmount, startDateMin, startDateMax,
                endDateMin, endDateMax, insuranceCompanyName, description);
            var ids = query.Select(x => x.Id);
            await DeleteManyAsync(ids, cancellationToken: cancellationToken);
        }

        public async virtual Task<long> GetCountAsync(
            string? filterText = null,
            string? policyNumber = null,
            decimal? premiumAmount = null,
            decimal? coverageAmount = null,
            DateTime? startDateMin = null,
            DateTime? startDateMax = null,
            DateTime? endDateMin = null,
            DateTime? endDateMax = null,
            EnumInsuranceCompanyName? insuranceCompanyName = null,
            string? description = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryableAsync(), filterText, policyNumber, premiumAmount, coverageAmount, startDateMin, startDateMax,
                endDateMin, endDateMax, insuranceCompanyName, description);
            return await query.LongCountAsync(cancellationToken);
        }

        protected virtual IQueryable<Insurance> ApplyFilter(
            IQueryable<Insurance> query,
            string? filterText = null,
            string? policyNumber = null,
            decimal? premiumAmount = null,
            decimal? coverageAmount = null,
            DateTime? startDateMin = null,
            DateTime? startDateMax = null,
            DateTime? endDateMin = null,
            DateTime? endDateMax = null,
            EnumInsuranceCompanyName? insuranceCompanyName = null,
            string? description = null) =>
                query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.PolicyNumber!.ToLower().Contains(filterText!.ToLower()) || e.Description!.ToLower().Contains(filterText!.ToLower()))
                    .WhereIf(!string.IsNullOrWhiteSpace(policyNumber), e => e.PolicyNumber!.ToLower().Contains(policyNumber!.ToLower()))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description!.ToLower().Contains(description!.ToLower()))
                    .WhereIf(startDateMin.HasValue, e => e.StartDate >= startDateMin!.Value)
                    .WhereIf(startDateMax.HasValue, e => e.StartDate <= startDateMax!.Value)
                    .WhereIf(endDateMin.HasValue, e => e.EndDate >= endDateMin!.Value)
                    .WhereIf(endDateMax.HasValue, e => e.EndDate <= endDateMax!.Value)
                    .WhereIf(insuranceCompanyName.HasValue && insuranceCompanyName != EnumInsuranceCompanyName.None, e => e.InsuranceCompanyName == insuranceCompanyName);
    }
}
