using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Patients;
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
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();

        query = ApplyFilter(query, filterText, type, startTimeMin, startTimeMax, endTime, patientId, departmentId);

        var ids = query.Select(x => x.Protocol.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<ProtocolWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        return (await GetDbSetAsync()).Where(b => b.Id == id)
            .Select(protocol => new ProtocolWithNavigationProperties
            {
                Protocol = protocol,
                Patient = dbContext.Set<Patient>().FirstOrDefault(c => c.Id == protocol.PatientId)!,
                Department = dbContext.Set<Department>().FirstOrDefault(c => c.Id == protocol.DepartmentId)!
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
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, type, startTimeMin, startTimeMax, endTime, patientId, departmentId);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ProtocolConsts.GetDefaultSorting(true) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    protected virtual async Task<IQueryable<ProtocolWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
    {
        return from protocol in (await GetDbSetAsync())
               join patient in (await GetDbContextAsync()).Set<Patient>() on protocol.PatientId equals patient.Id into patients
               from patient in patients.DefaultIfEmpty()
               join department in (await GetDbContextAsync()).Set<Department>() on protocol.DepartmentId equals department.Id into departments
               from department in departments.DefaultIfEmpty()
               select new ProtocolWithNavigationProperties
               {
                   Protocol = protocol,
                   Patient = patient,
                   Department = department
               };
    }

    protected virtual IQueryable<ProtocolWithNavigationProperties> ApplyFilter(
        IQueryable<ProtocolWithNavigationProperties> query,
        string? filterText,
        string? type = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        string? endTime = null,
        Guid? patientId = null,
        Guid? departmentId = null)
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Protocol.Type!.Contains(filterText!) || e.Protocol.EndTime!.Contains(filterText!))
                .WhereIf(!string.IsNullOrWhiteSpace(type), e => e.Protocol.Type.Contains(type!))
                .WhereIf(startTimeMin.HasValue, e => e.Protocol.StartTime >= startTimeMin!.Value)
                .WhereIf(startTimeMax.HasValue, e => e.Protocol.StartTime <= startTimeMax!.Value)
                .WhereIf(!string.IsNullOrWhiteSpace(endTime), e => e.Protocol.EndTime != null && e.Protocol.EndTime.Contains(endTime!))
                .WhereIf(patientId != null && patientId != Guid.Empty, e => e.Patient != null && e.Patient.Id == patientId)
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
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, type, startTimeMin, startTimeMax, endTime, patientId, departmentId);
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