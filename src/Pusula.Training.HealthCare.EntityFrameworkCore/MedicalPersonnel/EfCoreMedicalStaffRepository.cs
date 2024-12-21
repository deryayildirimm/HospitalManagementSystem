using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Districts;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.MedicalPersonnel;

public class EfCoreMedicalStaffRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, MedicalStaff, Guid>(dbContextProvider), IMedicalStaffRepository
{
    public virtual async Task DeleteAllAsync(
        string? filterText = null, 
        string? firstName = null, 
        string? lastName = null,
        string? identityNumber = null, 
        DateTime? birthDateMin = null, 
        DateTime? birthDateMax = null,
        EnumGender? gender = null, 
        string? email = null, 
        string? phoneNumber = null, 
        int? yearOfExperienceMin = 0,
        Guid? cityId = null, 
        Guid? districtId = null, 
        Guid? departmentId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, firstName, lastName, identityNumber, birthDateMin, birthDateMax, gender, email, phoneNumber, yearOfExperienceMin, cityId, districtId, departmentId);
        var ids = query.Select(x => x.MedicalStaff.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<MedicalStaffWithNavigationProperties> GetWithNavigationPropertiesAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        return (await GetDbSetAsync()).Where(b => b.Id == id)
            .Select(medicalStaff => new MedicalStaffWithNavigationProperties
            {
                MedicalStaff = medicalStaff,
                City = medicalStaff.City,
                District = medicalStaff.District,
                Department = medicalStaff.Department,
            })
            .FirstOrDefault()!;
    }

    public virtual async Task<List<MedicalStaffWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
        string? filterText = null, 
        string? firstName = null, 
        string? lastName = null,
        string? identityNumber = null, 
        DateTime? birthDateMin = null, 
        DateTime? birthDateMax = null,
        EnumGender? gender = null, 
        string? email = null, 
        string? phoneNumber = null, 
        int? yearOfExperienceMin = 0,
        Guid? cityId = null, 
        Guid? districtId = null, 
        Guid? departmentId = null, 
        string? sorting = null,
        int maxResultCount = int.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, firstName, lastName, identityNumber, birthDateMin, birthDateMax, gender, email, phoneNumber, yearOfExperienceMin, cityId, districtId, departmentId);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MedicalStaffConsts.GetDefaultSorting(true) : sorting);

        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<List<MedicalStaff>> GetListAsync(
        string? filterText = null, 
        string? firstName = null, 
        string? lastName = null,
        string? identityNumber = null, 
        DateTime? birthDateMin = null, 
        DateTime? birthDateMax = null,
        EnumGender? gender = null, 
        string? email = null, 
        string? phoneNumber = null, 
        int? yearOfExperienceMin = 0,
        Guid? cityId = null, 
        Guid? districtId = null, 
        Guid? departmentId = null, 
        string? sorting = null,
        int maxResultCount = int.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, firstName, lastName, identityNumber, birthDateMin, birthDateMax, gender, email, phoneNumber, yearOfExperienceMin, cityId, districtId, departmentId);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MedicalStaffConsts.GetDefaultSorting(false) : sorting);
        
        return await query.Page(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? filterText = null, 
        string? firstName = null, 
        string? lastName = null,
        string? identityNumber = null, 
        DateTime? birthDateMin = null, 
        DateTime? birthDateMax = null,
        EnumGender? gender = null, 
        string? email = null, 
        string? phoneNumber = null, 
        int? yearOfExperienceMin = 0,
        Guid? cityId = null, 
        Guid? districtId = null, 
        Guid? departmentId = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, firstName, lastName, identityNumber, birthDateMin, birthDateMax, gender, email, phoneNumber, yearOfExperienceMin, cityId, districtId, departmentId);
        
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }
    
    #region ApplyFilter and Queryable
        protected virtual IQueryable<MedicalStaff> ApplyFilter(
            IQueryable<MedicalStaff> query,
            string? filterText = null, 
            string? firstName = null, 
            string? lastName = null,
            string? identityNumber = null, 
            DateTime? birthDateMin = null, 
            DateTime? birthDateMax = null,
            EnumGender? gender = null, 
            string? email = null, 
            string? phoneNumber = null, 
            int? yearOfExperienceMin = 0,
            Guid? cityId = null,
            Guid? districtId = null,
            Guid? departmentId = null) =>
            query
                .Where(e => EF.Functions.ILike(e.FirstName, $"%{filterText}%")
                            || EF.Functions.ILike(e.LastName, $"%{filterText}%") 
                            || EF.Functions.ILike(e.IdentityNumber, $"%{filterText}%")
                            || EF.Functions.ILike(e.Email!, $"%{filterText}%"))
                .Where(e => EF.Functions.ILike(e.FirstName, $"%{firstName}%"))
                .Where(e => EF.Functions.ILike(e.LastName, $"%{lastName}%"))
                .Where(e => EF.Functions.ILike(e.IdentityNumber, $"%{identityNumber}%"))
                .WhereIf(birthDateMin.HasValue, e => e.BirthDate >= birthDateMin!.Value)
                .WhereIf(birthDateMax.HasValue, e => e.BirthDate <= birthDateMax!.Value)
                .WhereIf(gender.HasValue, e => e.Gender == gender)
                .Where(e => EF.Functions.ILike(e.Email!, $"%{email}%"))
                .Where(e => EF.Functions.ILike(e.PhoneNumber!, $"%{phoneNumber}%"))
                .WhereIf(yearOfExperienceMin.HasValue, e => e.StartDate <= DateTime.Now.AddYears(-yearOfExperienceMin!.Value))
                .WhereIf(cityId.HasValue, e => e.CityId == cityId)
                .WhereIf(districtId.HasValue, e => e.DistrictId == districtId)
                .WhereIf(departmentId.HasValue, e => e.DepartmentId == departmentId);

        protected virtual async Task<IQueryable<MedicalStaffWithNavigationProperties>> GetQueryForNavigationPropertiesAsync() =>
            from medicalStaff in (await GetDbSetAsync())
            join city in (await GetDbContextAsync()).Set<City>() on medicalStaff.CityId equals city.Id into cities
            from city in cities.DefaultIfEmpty()
            join district in (await GetDbContextAsync()).Set<District>() on medicalStaff.DistrictId equals district.Id into districts
            from district in districts.DefaultIfEmpty()
            join department in (await GetDbContextAsync()).Set<Department>() on medicalStaff.DepartmentId equals department.Id into departments
            from department in departments.DefaultIfEmpty()
            select new MedicalStaffWithNavigationProperties
            {
                MedicalStaff = medicalStaff,
                City = city,
                District = district,
                Department = department
            };


        protected virtual IQueryable<MedicalStaffWithNavigationProperties> ApplyFilter(
            IQueryable<MedicalStaffWithNavigationProperties> query,
            string? filterText = null, 
            string? firstName = null, 
            string? lastName = null,
            string? identityNumber = null, 
            DateTime? birthDateMin = null, 
            DateTime? birthDateMax = null,
            EnumGender? gender = null, 
            string? email = null, 
            string? phoneNumber = null, 
            int? yearOfExperienceMin = 0,
            Guid? cityId = null,
            Guid? districtId = null,
            Guid? departmentId = null) =>
                query
                    .Where(e => EF.Functions.ILike(e.MedicalStaff.FirstName, $"%{filterText}%")
                                || EF.Functions.ILike(e.MedicalStaff.LastName, $"%{filterText}%") 
                                || EF.Functions.ILike(e.MedicalStaff.IdentityNumber, $"%{filterText}%")
                                || EF.Functions.ILike(e.MedicalStaff.Email!, $"%{filterText}%"))
                    .Where(e => EF.Functions.ILike(e.MedicalStaff.FirstName, $"%{firstName}%"))
                    .Where(e => EF.Functions.ILike(e.MedicalStaff.LastName, $"%{lastName}%"))
                    .Where(e => EF.Functions.ILike(e.MedicalStaff.IdentityNumber, $"%{identityNumber}%"))
                    .WhereIf(birthDateMin.HasValue, e => e.MedicalStaff.BirthDate >= birthDateMin!.Value)
                    .WhereIf(birthDateMax.HasValue, e => e.MedicalStaff.BirthDate <= birthDateMax!.Value)
                    .WhereIf(gender.HasValue, e => e.MedicalStaff.Gender == gender)
                    .Where(e => EF.Functions.ILike(e.MedicalStaff.Email!, $"%{email}%"))
                    .Where(e => EF.Functions.ILike(e.MedicalStaff.PhoneNumber!, $"%{phoneNumber}%"))
                    .WhereIf(yearOfExperienceMin.HasValue, e => e.MedicalStaff.StartDate <= DateTime.Now.AddYears(-yearOfExperienceMin!.Value))
                    .WhereIf(cityId.HasValue, e => e.City.Id == cityId)
                    .WhereIf(districtId.HasValue, e => e.District.Id == districtId)
                    .WhereIf(departmentId.HasValue, e => e.Department.Id == departmentId);
        #endregion
}