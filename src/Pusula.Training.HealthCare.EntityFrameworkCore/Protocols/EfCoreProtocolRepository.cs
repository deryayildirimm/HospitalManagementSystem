
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.Protocols;

public class EfCoreProtocolRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider) 
    : EfCoreRepository<HealthCareDbContext, Protocol, Guid>(dbContextProvider), IProtocolRepository
{
    public virtual async Task DeleteAllAsync(
        string? filterText = null,
        string? type = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        string? endTime = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();

        query = ApplyFilter(query, filterText, type, startTimeMin, startTimeMax, endTime, patientId, departmentId, protocolTypeId);

        var ids = query.Select(x => x.Protocol.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<ProtocolWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
    {

        return (await GetDbSetAsync()).Where(b => b.Id == id)
            .Select(protocol => new ProtocolWithNavigationProperties
            {
                Protocol = protocol,
                Patient = protocol.Patient,
                Department = protocol.Department,
                ProtocolType = protocol.ProtocolType,
            }).FirstOrDefault()!;
    }

    public virtual async Task<List<ProtocolWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
        string? filterText = null,
        string? type = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        string? endTime = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, type, startTimeMin, startTimeMax, endTime, patientId, departmentId, protocolTypeId);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ProtocolConsts.GetDefaultSorting(true) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    protected virtual async Task<IQueryable<ProtocolWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
    {
        var protocols = await GetDbSetAsync();

        var query = from protocol in protocols
            select new ProtocolWithNavigationProperties
            {
                Protocol = protocol,
                Patient = protocol.Patient, 
                Department = protocol.Department ,
                ProtocolType = protocol.ProtocolType,
            };

        return query;
    }

    protected virtual IQueryable<ProtocolWithNavigationProperties> ApplyFilter(
        IQueryable<ProtocolWithNavigationProperties> query,
        string? filterText,
        string? type = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        string? endTime = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null)
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Protocol.Type!.Contains(filterText!) || e.Protocol.EndTime!.Contains(filterText!))
                .WhereIf(!string.IsNullOrWhiteSpace(type), e => e.Protocol.Type.Contains(type!))
                .WhereIf(startTimeMin.HasValue, e => e.Protocol.StartTime >= startTimeMin!.Value)
                .WhereIf(startTimeMax.HasValue, e => e.Protocol.StartTime <= startTimeMax!.Value)
                .WhereIf(!string.IsNullOrWhiteSpace(endTime), e => e.Protocol.EndTime != null && e.Protocol.EndTime.Contains(endTime!))
                .WhereIf(patientId != null && patientId != Guid.Empty, e => e.Patient != null && e.Patient.Id == patientId)
            .WhereIf(protocolTypeId != null && protocolTypeId != Guid.Empty, e => e.Protocol != null && e.Protocol.Id == protocolTypeId)
                .WhereIf(departmentId != null && departmentId != Guid.Empty, e => e.Department != null && e.Department.Id == departmentId);
    }

    public virtual async Task<List<Protocol>> GetListAsync(
        string? filterText = null,
        string? type = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        string? endTime = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, type, startTimeMin, startTimeMax, endTime);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ProtocolConsts.GetDefaultSorting(false) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? filterText = null,
        string? type = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        string? endTime = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, type, startTimeMin, startTimeMax, endTime, patientId, departmentId, protocolTypeId);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<Protocol> ApplyFilter(
        IQueryable<Protocol> query,
        string? filterText = null,
        string? type = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        string? endTime = null)
    {
        return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Type!.Contains(filterText!) || e.EndTime!.Contains(filterText!))
                .WhereIf(!string.IsNullOrWhiteSpace(type), e => e.Type.Contains(type!))
                .WhereIf(startTimeMin.HasValue, e => e.StartTime >= startTimeMin!.Value)
                .WhereIf(startTimeMax.HasValue, e => e.StartTime <= startTimeMax!.Value)
                .WhereIf(!string.IsNullOrWhiteSpace(endTime), e => e.EndTime != null && e.EndTime.Contains(endTime!));
    }
}