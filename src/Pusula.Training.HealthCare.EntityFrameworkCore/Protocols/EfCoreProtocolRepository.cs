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
using Pusula.Training.HealthCare.Permissions;

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

        query = ApplyFilter(query, filterText, note, startTimeMin, startTimeMax, endTimeMin, endTimeMax, patientId,
            departmentId, protocolTypeId, doctorId, insuranceId);

        var ids = query.Select(x => x.Protocol.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<Protocol> GetWithNavigationPropertiesAsync
        (Guid id, CancellationToken cancellationToken = default) =>
        await (await GetQueryableAsync())
            .Include(b => b.ProtocolType)
            .Include(b => b.Patient)
            .Include(b => b.Department)
            .Include(b => b.Doctor)
            .Include(b => b.Insurance)
            .FirstOrDefaultAsync(b => b.Id == id, GetCancellationToken(cancellationToken))!;


    public virtual async Task<Protocol> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        await (await GetQueryableAsync())
            .Include(b => b.ProtocolType)
            .Include(b => b.Patient)
            .Include(b => b.Department)
            .Include(b => b.Doctor)
            .Include(b => b.Insurance)
            .FirstOrDefaultAsync(b => b.Id == id, GetCancellationToken(cancellationToken))!;


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
                Department = protocol.Department,
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
        Guid? insuranceId = null) => query
        .WhereIf(!string.IsNullOrWhiteSpace(filterText), x =>
            x.Department.Name.ToLower().Contains(filterText!.ToLower()) ||
            x.Doctor.FirstName.ToLower().Contains(filterText!.ToLower()) ||
            x.Doctor.LastName.ToLower().Contains(filterText!.ToLower()) ||
            x.Patient.FirstName.ToLower().Contains(filterText!.ToLower()) ||
            x.Patient.LastName.ToLower().Contains(filterText!.ToLower()) ||
            x.ProtocolType.Name.ToLower().Contains(filterText!.ToLower()))
        .WhereIf(startTimeMin.HasValue, e => e.Protocol.StartTime >= startTimeMin!.Value)
        .WhereIf(startTimeMax.HasValue, e => e.Protocol.StartTime <= startTimeMax!.Value)
        .WhereIf(endTimeMin.HasValue, e => e.Protocol.EndTime >= endTimeMin!.Value)
        .WhereIf(endTimeMax.HasValue, e => e.Protocol.EndTime <= endTimeMax!.Value)
        .WhereIf(patientId.HasValue && patientId != Guid.Empty, e => e.Patient != null && e.Patient.Id == patientId)
        .WhereIf(insuranceId.HasValue && insuranceId != Guid.Empty,
            e => e.Insurance != null && e.Insurance.Id == insuranceId)
        .WhereIf(protocolTypeId.HasValue && protocolTypeId != Guid.Empty,
            e => e.ProtocolType != null && e.ProtocolType.Id == protocolTypeId)
        .WhereIf(doctorId.HasValue && doctorId != Guid.Empty, e => e.Doctor != null && e.Doctor.Id == doctorId)
        .WhereIf(departmentId.HasValue && departmentId != Guid.Empty,
            e => e.Department != null && e.Department.Id == departmentId);


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
        var query = ApplyFilter((await GetQueryableAsync()), filterText, note, startTimeMin, startTimeMax, endTimeMin,
            endTimeMax);

        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ProtocolConsts.GetDefaultSorting(false) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<List<ProtocolWithMedicalService>> GetProtocolWithMedicalServiceAsync(
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    #region Department-Patient Report 
   
    public virtual async Task<long> GetGroupCountByDepartmentPatientAsync(
        string? departmentName = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        Guid? doctorId = null,
        Guid? insuranceId = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        DateTime? endTimeMin = null,
        DateTime? endTimeMax = null,
        CancellationToken cancellationToken = default 
        )
    {
        
        var query = (await GetQueryableAsync());
        query = ApplyFilterForReports(query, departmentName, patientId, departmentId, protocolTypeId, doctorId, insuranceId,  startTimeMin, startTimeMax, endTimeMin, endTimeMax);
        
        return await query
            .GroupBy(a => a.Department.Id)
            .LongCountAsync(cancellationToken);
            
       
    }

    // case -> departman bazlı kaç kişi gelmiş 
    public virtual async Task<List<DepartmentStatistic>> GetGroupByDepartmentPatientAsync(
        string? departmentName = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        Guid? doctorId = null,
        Guid? insuranceId = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        DateTime? endTimeMin = null,
        DateTime? endTimeMax = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    )
    {
        var query = (await GetQueryableAsync());
        query = ApplyFilterForReports(query, departmentName, patientId, departmentId, protocolTypeId, doctorId, insuranceId, startTimeMin, startTimeMax, endTimeMin, endTimeMax);
        
        // bu departmana kaç kişi gelmiş -> benzersiz olarak düşünmek lazım , bu nedenle distinct eklemek mantıklı 
        // kaldırdım distinct i kaç kişi gelmiş diyor yani kaç protokol açılmış işte 
        return await query
            .GroupBy(a => a.Department.Id)
            .Select(g => new DepartmentStatistic
            {
                DepartmentId = g.Key,
                DepartmentName = g.First().Department.Name,
                PatientCount = g.Select(p => p.PatientId).Count(), // benzersiz hasta sayısını elde edebilmek içim
            })
            .OrderBy(d => d.DepartmentName)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }
    #endregion
    
    
    #region Doctor bazlı patient listesi
    
    public virtual async Task<long> GetGPatientsCountByDoctorAsync(
        string? departmentName = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        Guid? doctorId = null,
        Guid? insuranceId = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        DateTime? endTimeMin = null,
        DateTime? endTimeMax = null,
        CancellationToken cancellationToken = default 
    )
    {
        
        var query = (await GetQueryableAsync());
        query = ApplyFilterForReports(query, departmentName, patientId, departmentId, protocolTypeId, doctorId, insuranceId,  startTimeMin, startTimeMax, endTimeMin, endTimeMax);
        
        return await query
            .GroupBy(a => a.Id)
            .LongCountAsync(cancellationToken);
        
    }
    
     public virtual async Task<List<ProtocolPatientDoctorListReport>> GetGPatientsByDoctorAsync(
        string? departmentName = null,
        string? filterText = null,
        string? note = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        Guid? doctorId = null,
        Guid? insuranceId = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        DateTime? endTimeMin = null,
        DateTime? endTimeMax = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    )
    {
        var query = await GetQueryForNavigationPropertiesAsync();

// Filtreleme işlemi
        query = ApplyFilter(query, filterText, note, startTimeMin, startTimeMax, endTimeMin, endTimeMax, patientId,
            departmentId, protocolTypeId, doctorId, insuranceId);

// Departman bazlı gruplama ve hasta sayısı hesaplama
       var patientList = await query
           .GroupBy( p => new 
           { 
              
              p.Patient.Id ,
              p.Patient.PatientNumber, // Hasta numarası
              p.Patient.FirstName,
              p.Patient.LastName,
              DoctorName = p.Protocol.Doctor.FirstName +" " + p.Protocol.Doctor.LastName
           })
           .Select( g => new ProtocolPatientDoctorListReport
           {
               PatientId = g.Key.Id,
               PatientNumber = g.Key.PatientNumber, // Görünmesi için
               FullName = g.Key.FirstName + " " + g.Key.LastName,
               DoctorName = g.Key.DoctorName, 
               ProtocolCount = g.Count(),
               LastVisit = g.Max(x => x.Protocol.StartTime)
               
           })
           .OrderByDescending(p => p.LastVisit) // Son ziyaret tarihine göre sırala
           .PageBy(skipCount, maxResultCount)
           .ToListAsync(cancellationToken); 
       
       return patientList;
    }
    
    #endregion
    
    
    #region Departman bazlı patient listesi
    
    // burada gruplama yapmışım ama kullanmıycam bunu galiba. 
    // diğer metodlar da çalışırsa sadeleştiricez sonra 
    // bi deniyoruz bakalım bir şey olacak mı 
      public virtual async Task<long> GetGPatientsCountByDepartmentAsync(
        string? departmentName = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        Guid? doctorId = null,
        Guid? insuranceId = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        DateTime? endTimeMin = null,
        DateTime? endTimeMax = null,
        CancellationToken cancellationToken = default 
    )
    {
        
        var query = (await GetQueryableAsync());
        query = ApplyFilterForReports(query, departmentName, patientId, departmentId, protocolTypeId, doctorId, insuranceId,  startTimeMin, startTimeMax, endTimeMin, endTimeMax);
        
        return await query
            .GroupBy(a => a.Department.Id)
            .LongCountAsync(cancellationToken);
        
    }
 
    public virtual async Task<List<ProtocolPatientDepartmentListReport>> GetGPatientsByDepartmentAsync(
        string? departmentName = null,
        string? filterText = null,
        string? note = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        Guid? doctorId = null,
        Guid? insuranceId = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        DateTime? endTimeMin = null,
        DateTime? endTimeMax = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    )
    {
        var query = await GetQueryForNavigationPropertiesAsync();

// Filtreleme işlemi
        query = ApplyFilter(query, filterText, note, startTimeMin, startTimeMax, endTimeMin, endTimeMax, patientId,
            departmentId, protocolTypeId, doctorId, insuranceId);

// Departman bazlı gruplama ve hasta sayısı hesaplama
       var patientList = await query
           .GroupBy( p => new 
           { 
              
              p.Patient.Id ,
              p.Patient.PatientNumber, // Hasta numarası
              p.Patient.FirstName,
              p.Patient.LastName,
              DepartmentName = p.Protocol.Department.Name 
           })
           .Select( g => new ProtocolPatientDepartmentListReport
           {
               PatientId = g.Key.Id,
               PatientNumber = g.Key.PatientNumber, // Görünmesi için
               FullName = g.Key.FirstName + " " + g.Key.LastName,
               DepartmentName = g.Key.DepartmentName, // Departman ismini döndürüyoruz
               ProtocolCount = g.Count(),
               LastVisit = g.Max(x => x.Protocol.StartTime)
               
           })
           .OrderByDescending(p => p.LastVisit) // Son ziyaret tarihine göre sırala
           .PageBy(skipCount, maxResultCount)
           .ToListAsync(cancellationToken); 
       
       return patientList;
    }
    
    #endregion
 
    #region Doctor-Patient Report 
    public virtual async Task<long> GetGroupCountByDoctorPatientAsync(
        string? departmentName = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        Guid? doctorId = null,
        Guid? insuranceId = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        DateTime? endTimeMin = null,
        DateTime? endTimeMax = null,
        CancellationToken cancellationToken = default 
    )
    {
        
        var query = (await GetQueryableAsync());
        query = ApplyFilterForReports(query, departmentName, patientId, departmentId, protocolTypeId, doctorId, insuranceId, startTimeMin, startTimeMax, endTimeMin, endTimeMax);

        return await query
            .GroupBy(a => a.Doctor.Id)
            .LongCountAsync(cancellationToken);
        
    }
   
    
    public virtual async Task<List<DoctorStatistics>> GetGroupByDoctorPatientAsync(
        string? departmentName = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        Guid? doctorId = null,
        Guid? insuranceId = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        DateTime? endTimeMin = null,
        DateTime? endTimeMax = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    )
    {
        var query = (await GetQueryableAsync());
        query = ApplyFilterForReports(query, departmentName, patientId, departmentId, protocolTypeId, doctorId, insuranceId, startTimeMin, startTimeMax, endTimeMin, endTimeMax);
        
        return await query
            .GroupBy(a => a.Doctor.Id)
            .Select(g => new DoctorStatistics
            {
                DoctorId = g.Key,
                DoctorName = g.First().Doctor.FirstName +" " + g.First().Doctor.LastName,
                PatientCount = g.Select(p => p.PatientId).Count(), 
                
            })
            .OrderBy(d => d.DoctorName)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }
    #endregion
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
        query = ApplyFilter(query, filterText, note, startTimeMin, startTimeMax, endTimeMin, endTimeMax, patientId,
            departmentId, protocolTypeId, doctorId);
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
            e => e.Note!.Contains(filterText!))
        .WhereIf(!string.IsNullOrWhiteSpace(note), e => e.Note.Contains(note!))
        .WhereIf(startTimeMin.HasValue, e => e.StartTime >= startTimeMin!.Value)
        .WhereIf(startTimeMax.HasValue, e => e.StartTime <= startTimeMax!.Value)
        .WhereIf(endTimeMin.HasValue, e => e.EndTime >= endTimeMin!.Value)
        .WhereIf(endTimeMax.HasValue, e => e.EndTime <= endTimeMax!.Value);


    #region For department filter part
    
    protected virtual IQueryable<Protocol> ApplyFilterForReports(
        IQueryable<Protocol> query,
        string? departmentName = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        Guid? doctorId = null,
        Guid? insuranceId = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        DateTime? endTimeMin = null,
        DateTime? endTimeMax = null) => query
        .WhereIf(startTimeMin.HasValue, e => e.StartTime >= startTimeMin!.Value)
        .WhereIf(startTimeMax.HasValue, e => e.StartTime <= startTimeMax!.Value)
        .WhereIf(endTimeMin.HasValue, e => e.EndTime >= endTimeMin!.Value)
        .WhereIf(endTimeMax.HasValue, e => e.EndTime <= endTimeMax!.Value)
        .WhereIf(patientId.HasValue && patientId != Guid.Empty, e => e.Patient != null && e.Patient.Id == patientId)
        .WhereIf(insuranceId.HasValue && insuranceId != Guid.Empty,
            e => e.Insurance != null && e.Insurance.Id == insuranceId)
        .WhereIf(protocolTypeId.HasValue && protocolTypeId != Guid.Empty,
            e => e.ProtocolType != null && e.ProtocolType.Id == protocolTypeId)
        .WhereIf(doctorId.HasValue && doctorId != Guid.Empty, e => e.Doctor != null && e.Doctor.Id == doctorId)
        .WhereIf(!string.IsNullOrWhiteSpace(departmentName), x =>
            x.Department.Name.ToLower().Contains(departmentName!.ToLower()) )
        .WhereIf(departmentId.HasValue && departmentId != Guid.Empty,
            e => e.Department != null && e.Department.Id == departmentId);
    
    #endregion
    
}