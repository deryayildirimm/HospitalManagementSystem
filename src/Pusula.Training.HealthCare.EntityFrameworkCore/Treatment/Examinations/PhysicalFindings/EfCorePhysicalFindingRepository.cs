using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.Treatment.Examinations.PhysicalFindings;

public class EfCorePhysicalFindingRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, PhysicalFinding, Guid>(dbContextProvider), IPhysicalFindingRepository
{
    public virtual async Task<List<PhysicalFinding>> GetListAsync(
        int? weightMin = null,
        int? weightMax = null,
        int? heightMin = null,
        int? heightMax = null,
        Guid? examinationId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()).Include(e => e.Examination), 
            weightMin, weightMax, heightMin, heightMax, examinationId);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<long> GetCountAsync(
        int? weightMin = null,
        int? weightMax = null,
        int? heightMin = null,
        int? heightMax = null,
        Guid? examinationId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetDbSetAsync()),weightMin, weightMax, heightMin, heightMax, 
            examinationId);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<PhysicalFinding> ApplyFilter(
        IQueryable<PhysicalFinding> query,
        int? weightMin = null,
        int? weightMax = null,
        int? heightMin = null,
        int? heightMax = null,
        Guid? examinationId = null)
    {
        return query
            .WhereIf(weightMin.HasValue, x => x.Weight >= weightMin)
            .WhereIf(weightMax.HasValue, x => x.Weight <= weightMax)
            .WhereIf(heightMin.HasValue, x => x.Height >= heightMin)
            .WhereIf(heightMax.HasValue, x => x.Height <= heightMax)
            .WhereIf(examinationId.HasValue, e => e.ExaminationId == examinationId);
    }
}