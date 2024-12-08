using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public class EfCoreExaminationRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, Examination, Guid>(dbContextProvider), IExaminationRepository
{
    public virtual async Task DeleteAllAsync(
        string? filterText = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        string? complaint = null,
        string? story = null,
        Guid? protocolId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filterText, dateMin, dateMax, complaint, story, protocolId);
        
        var ids = query.Select(x => x.Id).ToList();
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<Examination>> GetListAsync(
        string? filterText = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        string? complaint = null,
        string? story = null,
        Guid? protocolId = null, 
        string? sorting = null,
        int maxResultCount = Int32.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()).Include(e => e.Background)
            .Include(e => e.FamilyHistory)
            .Include(e => e.Protocol), filterText, dateMin, dateMax, complaint, story, protocolId);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? filterText = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        string? complaint = null,
        string? story = null,
        Guid? protocolId = null, 
        string? sorting = null,
        int maxResultCount = Int32.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetDbSetAsync()), filterText, dateMin, dateMax, complaint, story, protocolId);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<Examination?> GetWithNavigationPropertiesAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetQueryableAsync())
            .Include(e => e.Background)
            .Include(e => e.FamilyHistory)
            .Include(e => e.Protocol);

        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async Task<Examination?> GetByProtocolIdAsync(
        Guid? protocolId, 
        CancellationToken cancellationToken = default)
    {
        if (protocolId == null) return null;
        
        var query = ApplyFilter((await GetQueryableAsync()).Include(e => e.Background)
            .Include(e => e.FamilyHistory)
            .Include(e => e.Protocol), protocolId: protocolId);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    protected virtual IQueryable<Examination> ApplyFilter(
        IQueryable<Examination> query,
        string? filterText = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        string? complaint = null,
        string? story = null,
        Guid? protocolId = null)
    => query
            .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Complaint!.ToLower().Contains(filterText!.ToLower())
                                                                  || e.Story!.ToLower().Contains(filterText!.ToLower()))
            .WhereIf(dateMin.HasValue, e => e.Date >= dateMin!.Value)
            .WhereIf(dateMax.HasValue, e => e.Date <= dateMax!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(complaint), e => e.Complaint!.ToLower().Contains(complaint!.ToLower()))
            .WhereIf(!string.IsNullOrWhiteSpace(story), e => e.Story!.ToLower().Contains(story!.ToLower()))
            .WhereIf(protocolId.HasValue, e => e.ProtocolId == protocolId);
}