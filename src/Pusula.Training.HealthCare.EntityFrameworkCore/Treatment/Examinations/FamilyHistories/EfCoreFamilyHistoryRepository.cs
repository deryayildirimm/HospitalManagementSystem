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
    public virtual async Task<List<FamilyHistory>> GetListAsync(
        string? filterText = null,
        string? motherDisease = null,
        string? fatherDisease = null,
        string? sisterDisease = null,
        string? brotherDisease = null,
        bool? areParentsRelated = null,
        Guid? examinationId = null, 
        string? sorting = null,
        int maxResultCount = int.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()).Include(e => e.Examination), 
            filterText, motherDisease, fatherDisease, sisterDisease, brotherDisease, areParentsRelated, examinationId);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(GetCancellationToken(cancellationToken));
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
        int maxResultCount = int.MaxValue, 
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
            .Where(f => EF.Functions.ILike(f.MotherDisease!, $"%{filterText}%")
                    || EF.Functions.ILike(f.FatherDisease!, $"%{filterText}%")
                    || EF.Functions.ILike(f.SisterDisease!, $"%{filterText}%")
                    || EF.Functions.ILike(f.BrotherDisease!, $"%{filterText}%"))
            .Where(f => EF.Functions.ILike(f.MotherDisease!, $"%{motherDisease}%"))
            .Where(f => EF.Functions.ILike(f.FatherDisease!, $"%{fatherDisease}%"))
            .Where(f => EF.Functions.ILike(f.SisterDisease!, $"%{sisterDisease}%"))
            .Where(f => EF.Functions.ILike(f.BrotherDisease!, $"%{brotherDisease}%"))
            .WhereIf(examinationId.HasValue, e => e.ExaminationId == examinationId)
            .WhereIf(areParentsRelated.HasValue, f => f.AreParentsRelated == areParentsRelated);
    }
}