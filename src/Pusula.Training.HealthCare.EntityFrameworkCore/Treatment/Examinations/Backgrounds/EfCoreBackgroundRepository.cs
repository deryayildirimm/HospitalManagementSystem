using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;

public class EfCoreBackgroundRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, Background, Guid>(dbContextProvider), IBackgroundRepository
{
    public virtual async Task<List<Background>> GetListAsync(
        string? filterText = null,
        string? allergies = null,
        string? medications = null,
        string? habits = null,
        Guid? examinationId = null, 
        string? sorting = null,
        int maxResultCount = int.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, allergies, medications, habits, examinationId);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<long> GetCountAsync(
        string? filterText = null,
        string? allergies = null,
        string? medications = null,
        string? habits = null,
        Guid? examinationId = null, 
        string? sorting = null,
        int maxResultCount = int.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetDbSetAsync()).Include(e => e.Examination),
            filterText, allergies, medications, habits, examinationId);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<Background> ApplyFilter(
        IQueryable<Background> query,
        string? filterText = null,
        string? allergies = null,
        string? medications = null,
        string? habits = null,
        Guid? examinationId = null)
    {
        return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Allergies!.ToLower().Contains(filterText!.ToLower())
                                                                      || e.Medications!.ToLower().Contains(filterText!.ToLower())
                                                                      || e.Habits!.ToLower().Contains(filterText!.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(allergies), e => e.Allergies!.ToLower().Contains(allergies!.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(medications), e => e.Medications!.ToLower().Contains(medications!.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(habits), e => e.Habits!.ToLower().Contains(habits!.ToLower()))
                .WhereIf(examinationId.HasValue, e => e.ExaminationId == examinationId);
    }
}