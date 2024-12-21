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
using Pusula.Training.HealthCare.Titles;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.Doctors;

public class EfCoreDoctorRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, Doctor, Guid>(dbContextProvider), IDoctorRepository
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
        Guid? titleId = null, 
        Guid? departmentId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, firstName, lastName, identityNumber, birthDateMin, birthDateMax, gender, email, phoneNumber, yearOfExperienceMin, cityId, districtId, titleId, departmentId);
        var ids = query.Select(x => x.Doctor.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }
    
    public virtual async Task<DoctorWithNavigationProperties> GetWithNavigationPropertiesAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        return (await GetDbSetAsync()).Where(b => b.Id == id)
            .Select(doctor => new DoctorWithNavigationProperties
            {
                Doctor = doctor,
                City = doctor.City,
                District = doctor.District,
                Title = doctor.Title,
                Department = doctor.Department,
            })
            .FirstOrDefault()!;
    }
    
    public virtual async Task<List<DoctorWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
        Guid? titleId = null, 
        Guid? departmentId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, firstName, lastName, identityNumber, birthDateMin, birthDateMax, gender, email, phoneNumber, yearOfExperienceMin, cityId, districtId, titleId, departmentId);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? DoctorConsts.GetDefaultSorting(true) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<List<Doctor>> GetListAsync(string? filterText = null, 
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
        Guid? titleId = null, 
        Guid? departmentId = null, 
        string? sorting = null,
        int maxResultCount = int.MaxValue, 
        int skipCount = 0, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, firstName, lastName, identityNumber, birthDateMin, birthDateMax, gender, email, phoneNumber, yearOfExperienceMin, cityId, districtId, titleId, departmentId);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? DoctorConsts.GetDefaultSorting(false) : sorting);
        
        return await query.Page(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(string? filterText = null, 
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
        Guid? titleId = null, 
        Guid? departmentId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, firstName, lastName, identityNumber, birthDateMin, birthDateMax, gender, email, phoneNumber, yearOfExperienceMin, cityId, districtId, titleId, departmentId);
        
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }
    
    public async Task<long> GetCountByDepartmentIdsAsync(
        string? filterText = null,
        List<Guid>? departmentIds = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, departmentIds: departmentIds);

        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }
    
    public virtual async Task<List<DoctorWithNavigationProperties>> GetListByDepartmentIdsAsync(
        string? filterText = null,
        List<Guid>? departmentIds = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, departmentIds: departmentIds);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? DoctorConsts.GetDefaultSorting(true) : sorting);

        return await query
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }
    
    #region ApplyFilter and Queryable
        protected virtual IQueryable<Doctor> ApplyFilter(
            IQueryable<Doctor> query,
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
            Guid? titleId = null, 
            Guid? departmentId = null,
            List<Guid>? departmentIds = null) =>
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
                .WhereIf(titleId.HasValue, e => e.TitleId == titleId)
                .WhereIf(departmentId.HasValue, e => e.DepartmentId == departmentId)
                .WhereIf(departmentIds != null && departmentIds.Any(), e => departmentIds!.Contains(e.DepartmentId));

        protected virtual async Task<IQueryable<DoctorWithNavigationProperties>> GetQueryForNavigationPropertiesAsync() =>
            from doctor in (await GetDbSetAsync())
            join city in (await GetDbContextAsync()).Set<City>() on doctor.CityId equals city.Id into cities
            from city in cities.DefaultIfEmpty()
            join district in (await GetDbContextAsync()).Set<District>() on doctor.DistrictId equals district.Id into districts
            from district in districts.DefaultIfEmpty()
            join title in (await GetDbContextAsync()).Set<Title>() on doctor.TitleId equals title.Id into titles
            from title in titles.DefaultIfEmpty()
            join department in (await GetDbContextAsync()).Set<Department>() on doctor.DepartmentId equals department.Id into departments
            from department in departments.DefaultIfEmpty()
            select new DoctorWithNavigationProperties
            {
                Doctor = doctor,
                City = city,
                District = district,
                Title = title,
                Department = department
            };


        protected virtual IQueryable<DoctorWithNavigationProperties> ApplyFilter(
            IQueryable<DoctorWithNavigationProperties> query,
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
            Guid? titleId = null, 
            Guid? departmentId = null,
            List<Guid>? departmentIds = null) =>
                query
                    .Where(e => EF.Functions.ILike(e.Doctor.FirstName, $"%{filterText}%")
                                || EF.Functions.ILike(e.Doctor.LastName, $"%{filterText}%") 
                                || EF.Functions.ILike(e.Doctor.IdentityNumber, $"%{filterText}%")
                                || EF.Functions.ILike(e.Doctor.Email!, $"%{filterText}%"))
                    .Where(e => EF.Functions.ILike(e.Doctor.FirstName, $"%{firstName}%"))
                    .Where(e => EF.Functions.ILike(e.Doctor.LastName, $"%{lastName}%"))
                    .Where(e => EF.Functions.ILike(e.Doctor.IdentityNumber, $"%{identityNumber}%"))
                    .WhereIf(birthDateMin.HasValue, e => e.Doctor.BirthDate >= birthDateMin!.Value)
                    .WhereIf(birthDateMax.HasValue, e => e.Doctor.BirthDate <= birthDateMax!.Value)
                    .WhereIf(gender.HasValue, e => e.Doctor.Gender == gender)
                    .Where(e => EF.Functions.ILike(e.Doctor.Email!, $"%{email}%"))
                    .Where(e => EF.Functions.ILike(e.Doctor.PhoneNumber!, $"%{phoneNumber}%"))
                    .WhereIf(yearOfExperienceMin.HasValue, e => e.Doctor.StartDate <= DateTime.Now.AddYears(-yearOfExperienceMin!.Value))
                    .WhereIf(cityId.HasValue, e => e.Doctor.CityId == cityId)
                    .WhereIf(districtId.HasValue, e => e.Doctor.DistrictId == districtId)
                    .WhereIf(titleId.HasValue, e => e.Title.Id == titleId)
                    .WhereIf(departmentId.HasValue, e => e.Department.Id == departmentId)
                    .WhereIf(departmentIds != null && departmentIds.Any(), e => departmentIds!.Contains(e.Doctor.DepartmentId));
        #endregion
}