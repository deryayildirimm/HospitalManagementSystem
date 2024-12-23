using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using Pusula.Training.HealthCare.Treatment.Icds;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public class EfCoreExaminationRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, Examination, Guid>(dbContextProvider), IExaminationRepository
{
    public virtual async Task<List<Examination>> GetListAsync(
        string? filterText = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        string? complaint = null,
        string? story = null,
        Guid? protocolId = null, 
        string? sorting = null,
        int maxResultCount = int.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter(await GetQueryForNavigationPropertiesAsync(
            true, 
            true, 
            true,
            true,
            true), filterText, dateMin, dateMax, complaint, story, protocolId);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ExaminationConsts.GetDefaultSorting(false) : sorting);

        return await query.PageBy(skipCount, maxResultCount).ToListAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<long> GetCountAsync(
        string? filterText = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        string? complaint = null,
        string? story = null,
        Guid? protocolId = null, 
        string? sorting = null,
        int maxResultCount = int.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetDbSetAsync()), filterText, dateMin, dateMax, complaint, story, protocolId);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }
    
    public virtual async Task<long> GetIcdReportCountAsync(
        DateTime startDate, 
        DateTime? endDate = null, 
        string? filterText = null,
        string? codeNumber = null,
        string? detail = null,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetQueryForNavigationPropertiesAsync(includeExaminationIcd: true))
            .SelectMany(e => e.ExaminationIcds);
        
        query = ApplyFilterForExaminationIcd(query, startDate, endDate, filterText, codeNumber, detail);
            
        var report = await query
            .GroupBy(icd => new { icd.Icd.CodeNumber, icd.Icd.Detail })
            .Select(group => new IcdReport(
                group.Key.CodeNumber, 
                group.Key.Detail,
                group.Count()))
            .LongCountAsync(GetCancellationToken(cancellationToken));
        return report;
    }
    
    public virtual async Task<List<IcdReport>> GetIcdReportAsync(
        DateTime startDate,
        DateTime? endDate = null,
        string? filterText = null,
        string? codeNumber = null,
        string? detail = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetQueryForNavigationPropertiesAsync(includeExaminationIcd: true))
            .SelectMany(e => e.ExaminationIcds);

        query = ApplyFilterForExaminationIcd(query, startDate, endDate, filterText, codeNumber, detail);
            
        var report = query
            .GroupBy(icd => new { icd.Icd.CodeNumber, icd.Icd.Detail })
            .Select(group => new IcdReport(
                group.Key.CodeNumber, 
                group.Key.Detail,
                group.Count()));
        
        return await report.PageBy(skipCount, maxResultCount).ToListAsync(GetCancellationToken(cancellationToken));
    }


    public virtual async Task<Examination?> GetWithNavigationPropertiesAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync(
            includeProtocol: true,
            includeFamilyHistory: true,
            includeBackground: true,
            includePhysicalFindings: true,
            includeExaminationIcd: true);

        return await query.FirstOrDefaultAsync(e => e.Id == id,
            cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<Examination?> GetByProtocolIdAsync(
        Guid? protocolId, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter(await GetQueryForNavigationPropertiesAsync(
            includeProtocol: true,
            includeFamilyHistory: true,
            includeBackground: true,
            includePhysicalFindings: true,
            includeExaminationIcd: true), protocolId: protocolId);
        return await query.FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }
    
    public virtual async Task UpdateExaminationIcdsAsync(Guid examinationId, List<Guid> icdIds, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var existingExaminationIcds = await dbContext.Set<ExaminationIcd>()
            .Where(ei => ei.ExaminationId == examinationId)
            .ToListAsync(cancellationToken);

        var toRemove = existingExaminationIcds.Where(ei => !icdIds.Contains(ei.IcdId)).ToList();
        dbContext.Set<ExaminationIcd>().RemoveRange(toRemove);

        var toAdd = icdIds
            .Where(icdId => !existingExaminationIcds.Any(ei => ei.IcdId == icdId))
            .Select(icdId => new ExaminationIcd(examinationId, icdId))
            .ToList();
        await dbContext.Set<ExaminationIcd>().AddRangeAsync(toAdd, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    #region NavigationQueryCreator

    protected virtual async Task<IQueryable<Examination>> GetQueryForNavigationPropertiesAsync(
        bool includeProtocol = false,
        bool includeFamilyHistory = false,
        bool includeBackground = false,
        bool includePhysicalFindings = false,
        bool includeExaminationIcd = false
        )
        =>
            (await GetQueryableAsync())
            .IncludeIf(includeProtocol, examination => examination.Protocol)
            .IncludeIf(includeFamilyHistory, examination => examination.FamilyHistory)
            .IncludeIf(includeBackground, examination => examination.Background)
            .IncludeIf(includePhysicalFindings, examination => examination.PhysicalFinding)
            .IncludeIf(includeExaminationIcd, examination => examination.ExaminationIcds);

    #endregion

    
    #region ApplyFilter
    protected virtual IQueryable<Examination> ApplyFilter(
        IQueryable<Examination> query,
        string? filterText = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        string? complaint = null,
        string? story = null,
        Guid? protocolId = null)
    => query
            .Where(e => EF.Functions.ILike(e.Complaint, $"%{filterText}%")
                || EF.Functions.ILike(e.Story!, $"%{filterText}%"))
            .WhereIf(dateMin.HasValue, e => e.Date >= dateMin!.Value)
            .WhereIf(dateMax.HasValue, e => e.Date <= dateMax!.Value)
            .Where(e => EF.Functions.ILike(e.Complaint, $"%{complaint}%"))
            .Where(e => EF.Functions.ILike(e.Story!, $"%{story}%"))
            .WhereIf(protocolId.HasValue, e => e.ProtocolId == protocolId);
    
    protected virtual IQueryable<ExaminationIcd> ApplyFilterForExaminationIcd(
        IQueryable<ExaminationIcd> query,
        DateTime startDate,
        DateTime? endDate = null,
        string? filterText = null,
        string? codeNumber = null,
        string? detail = null)
        => query
            .Where(icd => icd.Examination.Date >= startDate)
            .WhereIf(endDate.HasValue, icd => icd.Examination.Date <= endDate!.Value)
            .Where(icd => EF.Functions.ILike(icd.Icd.CodeNumber, $"%{filterText}%") 
                          || EF.Functions.ILike(icd.Icd.Detail, $"%{filterText}%") )
            .Where(icd => EF.Functions.ILike(icd.Icd.CodeNumber, $"%{codeNumber}%") )
            .Where(icd => EF.Functions.ILike(icd.Icd.Detail, $"%{detail}%") );

    
    #endregion
}