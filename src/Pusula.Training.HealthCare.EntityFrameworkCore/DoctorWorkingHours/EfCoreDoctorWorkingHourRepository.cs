using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;


namespace Pusula.Training.HealthCare.DoctorWorkingHours;

public class EfCoreDoctorWorkingHourRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, DoctorWorkingHour, Guid>(dbContextProvider), IDoctorWorkingHourRepository
{
    public virtual async Task<List<DoctorWorkingHour>> GetListAsync(
        Guid? doctorId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), doctorId);

        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting)
            ? DoctorWorkingHoursConsts.GetDefaultSorting(false)
            : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    protected virtual IQueryable<DoctorWorkingHour> ApplyFilter(
        IQueryable<DoctorWorkingHour> query,
        Guid? doctorId)
    {
        return query
            .WhereIf(doctorId.HasValue, e => e.DoctorId == doctorId);
    }
}