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

namespace Pusula.Training.HealthCare.Cities;

public class EfCoreCityRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, City, Guid>(dbContextProvider), ICityRepository
{
    public virtual async Task DeleteAllAsync(
        string? filterText = null, 
        string? name = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filterText, name);
        
        var ids = query.Select(x => x.Id).ToList();
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<City>> GetListAsync(
        string? filterText = null, 
        string? name = null, 
        string? sorting = null,
        int maxResultCount = Int32.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, name);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CityConsts.GetDefaultSorting(false) : sorting);
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
    
    protected virtual IQueryable<City> ApplyFilter(
        IQueryable<City> query,
        string? filterText = null,
        string? name = null)
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name!.ToLower().Contains(filterText!.ToLower()))
            .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.ToLower().Contains(name!.ToLower()));
    }
}