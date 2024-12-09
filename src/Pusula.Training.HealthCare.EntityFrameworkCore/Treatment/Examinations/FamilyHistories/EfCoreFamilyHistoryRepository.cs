using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;

public class EfCoreFamilyHistoryRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, FamilyHistory, Guid>(dbContextProvider), IFamilyHistoryRepository
{
    public virtual async Task DeleteAllAsync(
        string? filterText = null,
        string? motherDisease = null,
        string? fatherDisease = null,
        string? sisterDisease = null,
        string? brotherDisease = null,
        bool? areParentsRelated = null,
        Guid? examinationId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filterText, motherDisease, fatherDisease, sisterDisease, brotherDisease, areParentsRelated, examinationId);
        
        var ids = query.Select(x => x.Id).ToList();
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<FamilyHistory>> GetListAsync(
        string? filterText = null,
        string? motherDisease = null,
        string? fatherDisease = null,
        string? sisterDisease = null,
        string? brotherDisease = null,
        bool? areParentsRelated = null,
        Guid? examinationId = null, 
        string? sorting = null,
        int maxResultCount = Int32.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()).Include(e => e.Examination), 
            filterText, motherDisease, fatherDisease, sisterDisease, brotherDisease, areParentsRelated, examinationId);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? filterText = null,
        string? motherDisease = null,
        string? fatherDisease = null,
        string? sisterDisease = null,
        string? brotherDisease = null,
        bool? areParentsRelated = null,
        Guid? examinationId = null, 
        string? sorting = null,
        int maxResultCount = Int32.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetDbSetAsync()), filterText, motherDisease, fatherDisease, sisterDisease, brotherDisease, areParentsRelated, examinationId);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<FamilyHistory> ApplyFilter(
        IQueryable<FamilyHistory> query,
        string? filterText = null,
        string? motherDisease = null,
        string? fatherDisease = null,
        string? sisterDisease = null,
        string? brotherDisease = null,
        bool? areParentsRelated = null,
        Guid? examinationId = null)
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.MotherDisease!.ToLower().Contains(filterText!.ToLower())
                                || e.FatherDisease!.ToLower().Contains(filterText!.ToLower())
                                || e.SisterDisease!.ToLower().Contains(filterText!.ToLower())
                                || e.BrotherDisease!.ToLower().Contains(filterText!.ToLower()))
            .WhereIf(!string.IsNullOrWhiteSpace(motherDisease), e => e.MotherDisease!.ToLower().Contains(motherDisease!.ToLower()))
            .WhereIf(!string.IsNullOrWhiteSpace(fatherDisease), e => e.FatherDisease!.ToLower().Contains(fatherDisease!.ToLower()))
            .WhereIf(!string.IsNullOrWhiteSpace(sisterDisease), e => e.SisterDisease!.ToLower().Contains(sisterDisease!.ToLower()))
            .WhereIf(!string.IsNullOrWhiteSpace(brotherDisease), e => e.BrotherDisease!.ToLower().Contains(brotherDisease!.ToLower()))
            .WhereIf(examinationId.HasValue, e => e.ExaminationId == examinationId);
    }
}