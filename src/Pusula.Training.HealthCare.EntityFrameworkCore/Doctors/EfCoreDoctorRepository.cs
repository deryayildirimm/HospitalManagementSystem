using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.Departments;
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
                Title = dbContext.Set<Title>().FirstOrDefault(c => c.Id == doctor.TitleId)!,
                Department = dbContext.Set<Department>().FirstOrDefault(c => c.Id == doctor.DepartmentId)!
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

        var tmp = query.PageBy(skipCount, maxResultCount).ToQueryString();

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
        int maxResultCount = Int32.MaxValue, 
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
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.FirstName!.ToLower().Contains(filterText!.ToLower()) || e.LastName!.ToLower().Contains(filterText!.ToLower()) || e.PhoneNumber!.Contains(filterText!))
                .WhereIf(!string.IsNullOrWhiteSpace(firstName), e => e.FirstName!.ToLower().Contains(firstName!.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(lastName), e => e.LastName!.ToLower().Contains(lastName!.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(identityNumber), e => e.IdentityNumber!.Contains(identityNumber!))
                .WhereIf(birthDateMin.HasValue, e => e.BirthDate >= birthDateMin!.Value)
                .WhereIf(birthDateMax.HasValue, e => e.BirthDate <= birthDateMax!.Value)
                .WhereIf(gender.HasValue, e => e.Gender == gender)
                .WhereIf(!string.IsNullOrWhiteSpace(email), e => e.Email!.Contains(email!))
                .WhereIf(!string.IsNullOrWhiteSpace(phoneNumber), e => e.PhoneNumber!.Contains(phoneNumber!))
                .WhereIf(yearOfExperienceMin.HasValue, e => e.StartDate <= DateTime.Now.AddYears(-yearOfExperienceMin!.Value))
                .WhereIf(cityId.HasValue, e => e.CityId == cityId)
                .WhereIf(districtId.HasValue, e => e.DistrictId == districtId)
                .WhereIf(titleId.HasValue, e => e.TitleId == titleId)
                .WhereIf(departmentId.HasValue, e => e.DepartmentId == departmentId)
                .WhereIf(departmentIds != null && departmentIds.Any(), e => departmentIds!.Contains(e.DepartmentId));

        protected virtual async Task<IQueryable<DoctorWithNavigationProperties>> GetQueryForNavigationPropertiesAsync() =>
            from doctor in (await GetDbSetAsync())
            join title in (await GetDbContextAsync()).Set<Title>() on doctor.TitleId equals title.Id into titles
            from title in titles.DefaultIfEmpty()
            join department in (await GetDbContextAsync()).Set<Department>() on doctor.DepartmentId equals department.Id into departments
            from department in departments.DefaultIfEmpty()
            select new DoctorWithNavigationProperties
            {
                Doctor = doctor,
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
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Doctor.FirstName!.ToLower().Contains(filterText!.ToLower()) 
                                                                          || e.Doctor.LastName!.ToLower().Contains(filterText!.ToLower()) 
                                                                          || e.Doctor.PhoneNumber!.Contains(filterText!) 
                                                                          || filterText!.ToLower().Contains(e.Doctor.LastName!.ToLower()) 
                                                                          || filterText!.ToLower().Contains(e.Doctor.FirstName!.ToLower()))
                    .WhereIf(!string.IsNullOrWhiteSpace(firstName), e => e.Doctor.FirstName!.ToLower().Contains(firstName!.ToLower()))
                    .WhereIf(!string.IsNullOrWhiteSpace(lastName), e => e.Doctor.LastName!.ToLower().Contains(lastName!.ToLower()))
                    .WhereIf(!string.IsNullOrWhiteSpace(identityNumber), e => e.Doctor.IdentityNumber!.Contains(identityNumber!))
                    .WhereIf(birthDateMin.HasValue, e => e.Doctor.BirthDate >= birthDateMin!.Value)
                    .WhereIf(birthDateMax.HasValue, e => e.Doctor.BirthDate <= birthDateMax!.Value)
                    .WhereIf(gender.HasValue, e => e.Doctor.Gender == gender)
                    .WhereIf(!string.IsNullOrWhiteSpace(email), e => e.Doctor.Email!.Contains(email!))
                    .WhereIf(!string.IsNullOrWhiteSpace(phoneNumber), e => e.Doctor.PhoneNumber!.Contains(phoneNumber!))
                    .WhereIf(yearOfExperienceMin.HasValue, e => e.Doctor.StartDate <= DateTime.Now.AddYears(-yearOfExperienceMin!.Value))
                    .WhereIf(cityId.HasValue, e => e.City.Id == cityId)
                    .WhereIf(districtId.HasValue, e => e.District.Id == districtId)
                    .WhereIf(titleId.HasValue, e => e.Title.Id == titleId)
                    .WhereIf(departmentId.HasValue, e => e.Department.Id == departmentId)
                    .WhereIf(departmentIds != null && departmentIds.Any(), e => departmentIds!.Contains(e.Doctor.DepartmentId));
        #endregion
}