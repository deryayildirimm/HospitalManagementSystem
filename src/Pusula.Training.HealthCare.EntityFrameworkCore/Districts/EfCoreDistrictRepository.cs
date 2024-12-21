using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using Pusula.Training.HealthCare.Cities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.Districts;

public class EfCoreDistrictRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, District, Guid>(dbContextProvider), IDistrictRepository
{
    public virtual async Task DeleteAllAsync(
        string? filterText = null, 
        string? name = null, 
        Guid? cityId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, name, cityId);
        var ids = query.Select(x => x.District.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }
    
    public virtual async Task<DistrictWithNavigationProperties> GetWithNavigationPropertiesAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        return (await GetDbSetAsync()).Where(b => b.Id == id)
            .Select(district => new DistrictWithNavigationProperties
            {
                District = district,
                City = dbContext.Set<City>().FirstOrDefault(c => c.Id == district.CityId)!
            })
            .FirstOrDefault()!;
    }
    
    public virtual async Task<List<DistrictWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
        string? filterText = null,
        string? name = null,
        Guid? cityId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, name, cityId);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? DistrictConsts.GetDefaultSorting(true) : sorting);

        var tmp = query.PageBy(skipCount, maxResultCount).ToQueryString();

        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<List<District>> GetListAsync(string? filterText = null,
        string? name = null,
        Guid? cityId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, name, cityId);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? DistrictConsts.GetDefaultSorting(false) : sorting);
        
        return await query.Page(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? filterText = null,
        string? name = null,
        Guid? cityId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, name, cityId);
        
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }
    
    #region ApplyFilter and Queryable

    protected virtual IQueryable<District> ApplyFilter(
            IQueryable<District> query,
            string? filterText = null, 
            string? name = null,
            Guid? cityId = null) =>
            query
                .Where(e => EF.Functions.ILike(e.Name, $"%{filterText}%"))
                .Where(e => EF.Functions.ILike(e.Name, $"%{name}%"))
                .WhereIf(cityId.HasValue, e => e.CityId.Equals(cityId));

    protected virtual async Task<IQueryable<DistrictWithNavigationProperties>> GetQueryForNavigationPropertiesAsync() =>
        from district in (await GetDbSetAsync())
        join city in (await GetDbContextAsync()).Set<City>() on district.CityId equals city.Id into cities
        from city in cities.DefaultIfEmpty()
        select new DistrictWithNavigationProperties
        {
            District = district,
            City = city
        };


    protected virtual IQueryable<DistrictWithNavigationProperties> ApplyFilter(
        IQueryable<DistrictWithNavigationProperties> query,
        string? filterText = null,
        string? name = null,
        Guid? cityId = null) =>
        query
            .Where(e => EF.Functions.ILike(e.District.Name, $"%{filterText}%"))
            .Where(e => EF.Functions.ILike(e.District.Name, $"%{name}%"))
            .WhereIf(cityId.HasValue, e => e.City.Id.Equals(cityId));

    #endregion
}