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

namespace Pusula.Training.HealthCare.Titles;

public class EfCoreTitleRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, Title, Guid>(dbContextProvider), ITitleRepository
{
    public virtual async Task DeleteAllAsync(string? filterText = null, 
        string? titleName = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filterText, titleName);
        
        var ids = query.Select(x => x.Id).ToList();
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<Title>> GetListAsync(
        string? filterText = null, 
        string? titleName = null, 
        string? sorting = null,
        int maxResultCount = Int32.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, titleName);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? TitleConsts.GetDefaultSorting(false) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? filterText = null, 
        string? titleName = null, 
        string? sorting = null,
        int maxResultCount = Int32.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetDbSetAsync()), filterText, titleName);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<Title> ApplyFilter(
        IQueryable<Title> query,
        string? filterText = null,
        string? titleName = null)
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TitleName!.ToLower().Contains(filterText!.ToLower()))
            .WhereIf(!string.IsNullOrWhiteSpace(titleName), e => e.TitleName.ToLower().Contains(titleName!.ToLower()));
    }
}