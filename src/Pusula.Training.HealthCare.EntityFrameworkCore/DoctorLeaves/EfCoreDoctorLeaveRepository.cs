using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public class EfCoreDoctorLeaveRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, DoctorLeave, Guid>(dbContextProvider), IDoctorLeaveRepository
{
    public virtual async Task DeleteAllAsync(
        string? filterText = null,
        Guid? doctorId = null,
        DateTime? startDateMin = null,
        DateTime? startDateMax = null,
        DateTime? endDateMin = null,
        DateTime? endDateMax = null,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        query = ApplyFilter(query, filterText, doctorId, startDateMin, startDateMax, endDateMin, endDateMax, reason);

        var ids = query.Select(x => x.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<DoctorLeave>> GetListAsync(
        string? filterText = null,
        Guid? doctorId = null,
        DateTime? startDateMin = null,
        DateTime? startDateMax = null,
        DateTime? endDateMin = null,
        DateTime? endDateMax = null,
        string? reason = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryForNavigationPropertiesAsync()), filterText, doctorId, startDateMin,
            startDateMax,
            endDateMin, endDateMax, reason);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting)
            ? DoctorLeaveConsts.GetDefaultSorting(false)
            : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? filterText = null,
        Guid? doctorId = null,
        DateTime? startDateMin = null,
        DateTime? startDateMax = null,
        DateTime? endDateMin = null,
        DateTime? endDateMax = null,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryForNavigationPropertiesAsync()), filterText, doctorId, startDateMin,
            startDateMax, endDateMin,
            endDateMax, reason);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    #region NavigationQueryCreator

    protected virtual async Task<IQueryable<DoctorLeave>> GetQueryForNavigationPropertiesAsync()
        =>
            (await GetQueryableAsync())
            .Include(leave => leave.Doctor)
            .Include(leave => leave.Doctor.Department)
            .Include(leave => leave.Doctor.Title);

    #endregion

    protected virtual IQueryable<DoctorLeave> ApplyFilter(
        IQueryable<DoctorLeave> query,
        string? filterText = null,
        Guid? doctorId = null,
        DateTime? startDateMin = null,
        DateTime? startDateMax = null,
        DateTime? endDateMin = null,
        DateTime? endDateMax = null,
        string? reason = null) => query
        .WhereIf(!string.IsNullOrWhiteSpace(filterText),
            e => e.Reason!.Contains(filterText!))
        .WhereIf(!string.IsNullOrWhiteSpace(reason), x =>
            x.Reason != null && EF.Functions.ILike(x.Reason, $"%{reason}%"))
        .WhereIf(startDateMin.HasValue, e => e.StartDate >= startDateMin!.Value)
        .WhereIf(startDateMax.HasValue, e => e.StartDate <= startDateMax!.Value)
        .WhereIf(endDateMin.HasValue, e => e.EndDate >= endDateMin!.Value)
        .WhereIf(endDateMax.HasValue, e => e.EndDate <= endDateMax!.Value)
        .WhereIf(doctorId.HasValue, e => e.DoctorId == doctorId);
}