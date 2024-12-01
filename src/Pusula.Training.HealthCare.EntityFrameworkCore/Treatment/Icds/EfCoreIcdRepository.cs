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

namespace Pusula.Training.HealthCare.Treatment.Icds;

public class EfCoreIcdRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, Icd, Guid>(dbContextProvider), IIcdRepository
{
    public virtual async Task DeleteAllAsync(
        string? filterText = null,
        string? codeNumber = null, 
        string? detail = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filterText, codeNumber, detail);
        
        var ids = query.Select(x => x.Id).ToList();
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<Icd>> GetListAsync(
        string? filterText = null,
        string? codeNumber = null, 
        string? detail = null, 
        string? sorting = null, 
        int maxResultCount = Int32.MaxValue, 
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, codeNumber, detail);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? IcdConsts.GetDefaultSorting(false) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? filterText = null,
        string? codeNumber = null, 
        string? detail = null, 
        string? sorting = null, 
        int maxResultCount = Int32.MaxValue, 
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetDbSetAsync()), filterText, codeNumber, detail);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }
    
    protected virtual IQueryable<Icd> ApplyFilter(
        IQueryable<Icd> query,
        string? filterText = null,
        string? codeNumber = null, 
        string? detail = null)
    {
        if (!string.IsNullOrWhiteSpace(codeNumber))
        {
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAA");
        }
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filterText), e =>  e.CodeNumber.ToUpper().Contains(filterText!.ToUpper()) 
                                                                  || detail!.ToLower().Contains(filterText!.ToLower()))
            .WhereIf(!string.IsNullOrWhiteSpace(codeNumber), e => e.CodeNumber.ToUpper().Contains(codeNumber!.ToUpper()))
            .WhereIf(!string.IsNullOrWhiteSpace(detail), e => e.CodeNumber.ToLower().Contains(detail!.ToLower()));
    }
}