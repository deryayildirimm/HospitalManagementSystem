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

namespace Pusula.Training.HealthCare.Restrictions;

public class EfCoreRestrictionRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, Restriction, Guid>(dbContextProvider), IRestrictionRepository
{
    public virtual async Task<List<Restriction>> GetListAsync(
        Guid? medicalServiceId,
        Guid? departmentId = null,
        Guid? doctorId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter(
            (await GetQueryForNavigationPropertiesAsync()),
            medicalServiceId, departmentId, doctorId);

        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting)
            ? RestrictionConsts.GetDefaultSorting(false)
            : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        Guid? medicalServiceId,
        Guid? departmentId = null,
        Guid? doctorId = null,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryForNavigationPropertiesAsync()),
            medicalServiceId, departmentId, doctorId);

        return await query.LongCountAsync(cancellationToken);
    }


    protected virtual async Task<IQueryable<Restriction>> GetQueryForNavigationPropertiesAsync()
        =>
            (await GetQueryableAsync())
            .Include(restriction => restriction.Department)
            .Include(restriction => restriction.MedicalService)
            .Include(restriction => restriction.Doctor);

    #region ApplyFilter

    protected virtual IQueryable<Restriction> ApplyFilter(
        IQueryable<Restriction> query,
        Guid? medicalServiceId = null,
        Guid? departmentId = null,
        Guid? doctorId = null) =>
        query
            .WhereIf(doctorId.HasValue, x => x.DoctorId == doctorId)
            .WhereIf(departmentId.HasValue, x => x.DepartmentId == departmentId)
            .WhereIf(medicalServiceId.HasValue, x => x.MedicalServiceId == medicalServiceId);

    #endregion
}