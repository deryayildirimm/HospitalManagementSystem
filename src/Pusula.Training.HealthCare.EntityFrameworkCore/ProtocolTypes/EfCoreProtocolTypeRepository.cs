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

namespace Pusula.Training.HealthCare.ProtocolTypes;

public class EfCoreProtocolTypeRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, ProtocolType, Guid>(dbContextProvider), IProtocolTypeRepository
{
    public virtual async Task DeleteAllAsync(string? filterText = null, 
        string? name = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filterText, name);
        
        var ids = query.Select(x => x.Id).ToList();
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<ProtocolType>> GetListAsync(
        string? filterText = null, 
        string? name = null, 
        string? sorting = null,
        int maxResultCount = Int32.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, name);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ProtocolTypeConsts.GetDefaultSorting(false) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? filterText = null, 
        string? name = null, 
        string? sorting = null,
        int maxResultCount = Int32.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetDbSetAsync()), filterText, name);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<ProtocolType> ApplyFilter(
        IQueryable<ProtocolType> query,
        string? filterText = null,
        string? name = null)  => query
        .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => EF.Functions.ILike(e.Name!, $"%{filterText}%"))
        .WhereIf(!string.IsNullOrWhiteSpace(name), e => EF.Functions.ILike(e.Name!, $"%{name}%"));
    
}