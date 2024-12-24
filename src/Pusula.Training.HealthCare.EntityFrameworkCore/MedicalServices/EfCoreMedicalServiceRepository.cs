using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.MedicalServices;

public class EfCoreMedicalServiceRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, MedicalService, Guid>(dbContextProvider: dbContextProvider),
        IMedicalServiceRepository
{
    public async Task DeleteAllAsync(
        string? name = null,
        double? costMin = null,
        double? costMax = null,
        DateTime? serviceDateMin = null,
        DateTime? serviceDateMax = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        query = ApplyFilter(query, name, costMin, costMax, serviceDateMin, serviceDateMax);

        var ids = query.Select(x => x.Id).ToList();
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<MedicalService?> GetWithDetailsAsync(
        Guid medicalServiceId,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter(
            (await GetQueryForNavigationPropertiesAsync()),
            medicalServiceId: medicalServiceId);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<List<MedicalService>> GetListAsync(
        Guid? departmentId = null,
        string? name = null,
        double? costMin = null,
        double? costMax = null,
        DateTime? serviceDateMin = null,
        DateTime? serviceDateMax = null,
        string? sorting = null,
        int maxResultCount = 10,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()),
            medicalServiceId: null,
            departmentId: departmentId,
            name,
            costMin, costMax, serviceDateMin, serviceDateMax);

        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting)
            ? MedicalServiceConsts.GetDefaultSorting(false)
            : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<List<MedicalService>> GetMedicalServiceListByDepartmentIdAsync(
        Guid departmentId,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()),
            departmentId: departmentId);

        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting)
            ? MedicalServiceConsts.GetDefaultSorting(false)
            : sorting);

        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<List<MedicalServiceWithDepartments>> GetMedicalServiceWithDepartmentsAsync(
        string? name = null,
        double? costMin = null,
        double? costMax = null,
        DateTime? serviceDateMin = null,
        DateTime? serviceDateMax = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), name, costMin, costMax, serviceDateMin, serviceDateMax);

        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting)
            ? MedicalServiceConsts.GetDefaultSorting(false)
            : sorting);

        return await query.Select(ms => new MedicalServiceWithDepartments
        {
            MedicalService = ms,
            Departments = ms.DepartmentMedicalServices
                .Select(dms => dms.Department)
                .ToList()
        }).ToListAsync(cancellationToken: cancellationToken);
    }

    public virtual async Task<MedicalServiceWithDoctors> GetMedicalServiceWithDoctorsAsync(
        Guid medicalServiceId,
        Guid? departmentId,
        string? name = null,
        double? costMin = null,
        double? costMax = null,
        DateTime? serviceDateMin = null,
        DateTime? serviceDateMax = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter(
            (await GetQueryForNavigationPropertiesAsync()),
            medicalServiceId: medicalServiceId,
            departmentId: departmentId,
            name,
            costMin,
            costMax,
            serviceDateMin,
            serviceDateMax);

        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting)
            ? MedicalServiceConsts.GetDefaultSorting(false)
            : sorting);

        return (await query
            .Select(ms => new MedicalServiceWithDoctors
            {
                MedicalService = ms,
                Doctors = ms.DepartmentMedicalServices
                    .Select(dms => dms.Department)
                    .SelectMany(dept => dept.Doctors)
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken))!;
    }

    public virtual async Task<List<DoctorWithDetails>> GetMedicalServiceDoctorsAsync(
        Guid medicalServiceId,
        Guid departmentId,
        string? doctorFilterText = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyAdvancedFilter(
            (await GetQueryForNavigationPropertiesAsync()),
            medicalServiceId: medicalServiceId,
            departmentId: departmentId,
            doctorFilterText: doctorFilterText);

        var doctorQuery = query
            .Select(doctor => new DoctorWithDetails
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Gender = doctor.Gender,
                DepartmentId = doctor.DepartmentId,
                DepartmentName = doctor.Department.Name,
                TitleId = doctor.TitleId,
                TitleName = doctor.Title.TitleName
            });

        return await doctorQuery
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        Guid? departmentId = null,
        string? name = null,
        double? costMin = null,
        double? costMax = null,
        DateTime? serviceDateMin = null,
        DateTime? serviceDateMax = null,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetDbSetAsync()),
            medicalServiceId: null,
            departmentId: departmentId,
            name: name,
            costMin: costMin,
            costMax: costMax,
            serviceDateMin: serviceDateMin,
            serviceDateMax: serviceDateMax);

        return await query.LongCountAsync(cancellationToken);
    }

    #region NavigationQueryCreator

    protected virtual async Task<IQueryable<MedicalService>> GetQueryForNavigationPropertiesAsync()
        =>
            (await GetQueryableAsync())
            .Include(ms => ms.DepartmentMedicalServices)
            .ThenInclude(dms => dms.Department)
            .ThenInclude(dept => dept.Doctors)
            .ThenInclude(doc => doc.Title);

    #endregion

    #region Filters

    protected virtual IQueryable<MedicalService> ApplyFilter(
        IQueryable<MedicalService> query,
        string? name = null,
        double? costMin = null,
        double? costMax = null,
        DateTime? serviceDateMin = null,
        DateTime? serviceDateMax = null)
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name!))
            .WhereIf(costMin.HasValue,
                e => e.Cost >= costMin!.Value)
            .WhereIf(costMax.HasValue,
                e => e.Cost <= costMax!.Value)
            .WhereIf(serviceDateMin.HasValue, e => e.ServiceCreatedAt >= serviceDateMin!.Value)
            .WhereIf(serviceDateMax.HasValue, e => e.ServiceCreatedAt <= serviceDateMax!.Value);
    }

    protected virtual IQueryable<MedicalService> ApplyFilter(
        IQueryable<MedicalService> query,
        Guid? medicalServiceId = null,
        Guid? departmentId = null,
        string? name = null,
        double? costMin = null,
        double? costMax = null,
        DateTime? serviceDateMin = null,
        DateTime? serviceDateMax = null)
    {
        return query
            .WhereIf(medicalServiceId.HasValue, e => e.Id == medicalServiceId!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name!.Contains(name!))
            .WhereIf(departmentId.HasValue,
                e => e.DepartmentMedicalServices.Any(dms => dms.Department.Id == departmentId!.Value))
            .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name!.Contains(name!))
            .WhereIf(costMin.HasValue,
                e => e.Cost >= costMin!.Value)
            .WhereIf(costMax.HasValue,
                e => e.Cost <= costMax!.Value)
            .WhereIf(serviceDateMin.HasValue,
                e => e.ServiceCreatedAt >= serviceDateMin!.Value)
            .WhereIf(serviceDateMax.HasValue,
                e => e.ServiceCreatedAt <= serviceDateMax!.Value);
    }

    protected virtual IQueryable<Doctor> ApplyAdvancedFilter(
        IQueryable<MedicalService> query,
        Guid medicalServiceId,
        Guid departmentId,
        string? doctorFilterText = null)
    {
        return query
            .SelectMany(ms => ms.DepartmentMedicalServices)
            .Where(dms => dms.MedicalServiceId == medicalServiceId &&
                          dms.DepartmentId == departmentId)
            .Select(dms => dms.Department)
            .SelectMany(d => d.Doctors)
            .WhereIf(!string.IsNullOrWhiteSpace(doctorFilterText),
                doctor => EF.Functions.ILike(doctor.FirstName, $"%{doctorFilterText}%") ||
                          EF.Functions.ILike(doctor.LastName, $"%{doctorFilterText}%"))
            .Distinct();
    }

    #endregion
}