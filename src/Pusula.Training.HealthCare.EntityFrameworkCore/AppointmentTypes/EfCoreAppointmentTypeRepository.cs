using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.AppointmentTypes;

public class EfCoreAppointmentTypeRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, AppointmentType, Guid>(dbContextProvider), IAppointmentTypeRepository
{
    public virtual async Task<List<AppointmentType>> GetListAsync(
        string? filterText = null, string? name = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, name);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting)
            ? AppointmentTypeConsts.GetDefaultSorting(false)
            : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? filterText = null, 
        string? name = null,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetDbSetAsync()), filterText, name);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<AppointmentType> ApplyFilter(
        IQueryable<AppointmentType> query,
        string? filterText = null,
        string? name = null)
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name!.Contains(filterText!))
            .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name!));
    }
}