
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
        string? note = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        DateTime? endTimeMin = null,
        DateTime? endTimeMax = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        Guid? doctorId = null,
        Guid? insuranceId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();

        query = ApplyFilter(query, filterText, note, startTimeMin, startTimeMax, endTimeMin, endTimeMax, patientId, departmentId, protocolTypeId, doctorId, insuranceId);

        var ids = query.Select(x => x.Protocol.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<Protocol> GetWithNavigationPropertiesAsync
        (Guid id, CancellationToken cancellationToken = default)  =>  
        await (await GetQueryableAsync())
            .Include(b => b.ProtocolType)
            .Include(b => b.Patient)
            .Include(b => b.Department)
            .Include(b => b.Doctor)
            .Include(b => b.Insurance)
            .FirstOrDefaultAsync( b => b.Id == id,  GetCancellationToken(cancellationToken))!;



    public virtual async Task<Protocol> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
       await (await GetQueryableAsync())
        .Include(b => b.ProtocolType)
        .Include(b => b.Patient)
        .Include(b => b.Department)
        .Include(b => b.Doctor)
        .Include(b => b.Insurance)
        .FirstOrDefaultAsync( b => b.Id == id,  GetCancellationToken(cancellationToken))!;


    public virtual async Task<List<ProtocolWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
        string? filterText = null,
        string? note = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        DateTime? endTimeMin = null,
        DateTime? endTimeMax = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        Guid? doctorId = null,
        Guid? insuranceId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetQueryForNavigationPropertiesAsync());
        query = ApplyFilter(query, filterText, note, startTimeMin, startTimeMax, endTimeMin, endTimeMax, patientId,
            departmentId, protocolTypeId, doctorId, insuranceId);
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
                Doctor = protocol.Doctor,
                Insurance = protocol.Insurance
            };

        return query;
    }

    protected virtual IQueryable<ProtocolWithNavigationProperties> ApplyFilter(
        IQueryable<ProtocolWithNavigationProperties> query,
        string? filterText,
        string? note = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        DateTime? endTimeMin = null,
        DateTime? endTimeMax = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        Guid? doctorId = null,
        Guid? insuranceId = null)  =>  query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e =>
                            (e.Protocol.Note != null && string.Equals(e.Protocol.Note, filterText, StringComparison.OrdinalIgnoreCase)) ||
                             string.Equals(e.Department.Name , filterText , StringComparison.OrdinalIgnoreCase) ||  
                             string.Equals(e.Doctor.FirstName , filterText , StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(e.Doctor.LastName, filterText, StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(e.Patient.FirstName, filterText, StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(e.Patient.LastName, filterText, StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(e.ProtocolType.Name, filterText, StringComparison.OrdinalIgnoreCase)
                           )
                .WhereIf(startTimeMin.HasValue, e => e.Protocol.StartTime >= startTimeMin!.Value)
                .WhereIf(startTimeMax.HasValue, e => e.Protocol.StartTime <= startTimeMax!.Value)
                .WhereIf(endTimeMin.HasValue, e => e.Protocol.EndTime >= endTimeMin!.Value)
                .WhereIf(endTimeMax.HasValue, e => e.Protocol.EndTime <= endTimeMax!.Value)
                .WhereIf(patientId != null && patientId != Guid.Empty, e => e.Patient != null && e.Patient.Id == patientId)
                .WhereIf(insuranceId != null && insuranceId != Guid.Empty, e => e.Insurance != null && e.Insurance.Id == insuranceId)
                .WhereIf(protocolTypeId != null && protocolTypeId != Guid.Empty, e => e.ProtocolType != null && e.ProtocolType.Id == protocolTypeId)
                .WhereIf(doctorId != null && doctorId != Guid.Empty, e => e.Doctor != null && e.Doctor.Id == doctorId)
                .WhereIf(departmentId != null && departmentId != Guid.Empty, e => e.Department != null && e.Department.Id == departmentId);
    

    public virtual async Task<List<Protocol>> GetListAsync(
        string? filterText = null,
        string? note = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        DateTime? endTimeMin = null,
        DateTime? endTimeMax = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, note, startTimeMin, startTimeMax, endTimeMin,endTimeMax);
        
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ProtocolConsts.GetDefaultSorting(false) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? filterText = null,
        string? note = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        DateTime? endTimeMin = null,
        DateTime? endTimeMax = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        Guid? doctorId = null,
        Guid? insuranceId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, note, startTimeMin, startTimeMax, endTimeMin, endTimeMax, patientId, departmentId, protocolTypeId,doctorId );
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<Protocol> ApplyFilter(
        IQueryable<Protocol> query,
        string? filterText = null,
        string? note = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        DateTime? endTimeMin = null,
        DateTime? endTimeMax = null) => query
            .WhereIf(!string.IsNullOrWhiteSpace(filterText),
                e => e.Note!.Contains(filterText!) )
            .WhereIf(!string.IsNullOrWhiteSpace(note), e => e.Note.Contains(note!))
            .WhereIf(startTimeMin.HasValue, e => e.StartTime >= startTimeMin!.Value)
            .WhereIf(startTimeMax.HasValue, e => e.StartTime <= startTimeMax!.Value) 
            .WhereIf(endTimeMin.HasValue, e => e.EndTime >= endTimeMin!.Value)
            .WhereIf(endTimeMax.HasValue, e => e.EndTime <= endTimeMax!.Value);

    
}