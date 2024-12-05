using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.Patients;

public class EfCorePatientRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, Patient, Guid>(dbContextProvider), IPatientRepository
{
    public virtual async Task DeleteAllAsync(
        string? filterText = null,
        int? patientNumber = null,
        string? firstName = null,
        string? lastName = null,
        string? identityNumber = null,
        string? nationality = null,
        string? passportNumber = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        string? emailAddress = null,
        string? mobilePhoneNumber = null,
        EnumPatientTypes? patientType = null,
        EnumInsuranceType? insuranceType = null,
        string? insuranceNo = null,
        EnumDiscountGroup? discountGroup = null,
        EnumGender? gender = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        query = ApplyFilter(query, filterText, patientNumber, firstName, lastName, identityNumber, nationality, passportNumber, birthDateMin, birthDateMax, 
            emailAddress, mobilePhoneNumber, patientType, insuranceType, insuranceNo, discountGroup, gender);

        var ids = query.Select(x => x.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<Patient>> GetListAsync(
        string? filterText = null,
        int? patientNumber = null,
        string? firstName = null,
        string? lastName = null,
        string? identityNumber = null,
        string? nationality = null,
        string? passportNumber = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        string? emailAddress = null,
        string? mobilePhoneNumber = null,
        EnumPatientTypes? patientType = null,
        EnumInsuranceType? insuranceType = null,
        string? insuranceNo = null,
        EnumDiscountGroup? discountGroup = null,
        EnumGender? gender = null,
        string? sorting = null,
        bool? isDeleted = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, patientNumber, firstName, lastName, identityNumber, nationality, passportNumber, birthDateMin, 
            birthDateMax, emailAddress, mobilePhoneNumber, patientType, insuranceType, insuranceNo, discountGroup, gender, isDeleted);
      //  query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PatientConsts.GetDefaultSorting(false) : sorting);
      query = query.OrderBy(e => e.IsDeleted) // IsDeleted'e göre önce sıralama
          .ThenByDescending(e => e.CreationTime) // IsDeleted içindeki her grupta en son oluşturulan en başta görünsün
          .ThenBy(string.IsNullOrWhiteSpace(sorting) ? PatientConsts.GetDefaultSorting(false) : sorting); // Ardından mevcut sıralamayı uygula
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }
    
       
    public virtual async Task<Patient> GetPatientByNumberAsync(
        int patientNumber ,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        
        return await dbContext.Patients
                   .Where(a => a.PatientNumber == patientNumber)
                   .FirstOrDefaultAsync(cancellationToken)
               ?? throw new EntityNotFoundException(typeof(Patient), patientNumber);
    }
    
    public virtual async Task<Patient> GetPatientByInsuranceNumberAsync(
       string insuranceNumber,
        CancellationToken cancellationToken = default)
    {
        Check.NotNullOrWhiteSpace(insuranceNumber, nameof(insuranceNumber));
        var dbContext = await GetDbContextAsync();
        
        return await dbContext.Patients
                   .Where(a => a.InsuranceNo == insuranceNumber )
                   .FirstOrDefaultAsync(cancellationToken)
               ?? throw new EntityNotFoundException(typeof(Patient), insuranceNumber);
    }

    

    public virtual async Task<long> GetCountAsync(
        string? filterText = null,
        int? patientNumber = null,
        string? firstName = null,
        string? lastName = null,
        string? identityNumber = null,
        string? nationality = null,
        string? passportNumber = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        string? emailAddress = null,
        string? mobilePhoneNumber = null,
        EnumPatientTypes? patientType = null,
        EnumInsuranceType? insuranceType = null,
        string? insuranceNo = null,
        EnumDiscountGroup? discountGroup = null,
        EnumGender? gender = null,
        bool? isDeleted = null,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetDbSetAsync()), filterText, patientNumber, firstName, lastName, identityNumber, nationality, passportNumber, birthDateMin, birthDateMax, 
            emailAddress, mobilePhoneNumber, patientType, insuranceType, insuranceNo, discountGroup, gender, isDeleted);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<Patient> ApplyFilter(
        IQueryable<Patient> query,
        string? filterText = null,
        int? patientNumber = null,
        string? firstName = null,
        string? lastName = null,
        string? identityNumber = null,
        string? nationality = null,
        string? passportNumber = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        string? emailAddress = null,
        string? mobilePhoneNumber = null,
        EnumPatientTypes? patientType = null,
        EnumInsuranceType? insuranceType = null,
        string? insuranceNo = null,
        EnumDiscountGroup? discountGroup = null,
        EnumGender? gender = null, 
        bool? isDeleted = null)
    {
        // sadece isDeleted true ise silinen verileri de göster 
        query = isDeleted == true ? query.IgnoreQueryFilters() : query;
        
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filterText),
                e => e.FirstName!.Contains(filterText!) || e.LastName!.Contains(filterText!) ||
                     e.IdentityNumber!.Contains(filterText!) || e.EmailAddress!.Contains(filterText!) ||
                     e.MobilePhoneNumber!.Contains(filterText!))
            .WhereIf(!string.IsNullOrWhiteSpace(firstName), e => e.FirstName.ToLower().Contains(firstName!.ToLower()))
            .WhereIf(!string.IsNullOrWhiteSpace(lastName), e => e.LastName.ToLower().Contains(lastName!.ToLower()))
            .WhereIf(!string.IsNullOrWhiteSpace(identityNumber), e => e.IdentityNumber!.Contains(identityNumber!))
            .WhereIf(!string.IsNullOrWhiteSpace(passportNumber),
                e => e.PassportNumber!.ToLower().Contains(passportNumber!.ToLower()))
            .WhereIf(birthDateMin.HasValue, e => e.BirthDate >= birthDateMin!.Value)
            .WhereIf(birthDateMax.HasValue, e => e.BirthDate <= birthDateMax!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(emailAddress), e => e.EmailAddress!.Contains(emailAddress!))
            .WhereIf(!string.IsNullOrWhiteSpace(mobilePhoneNumber),
                e => e.MobilePhoneNumber.Contains(mobilePhoneNumber!))
            .WhereIf(patientType.HasValue, e => e.PatientType == patientType)
            .WhereIf(insuranceType.HasValue, e => e.InsuranceType == insuranceType)
            .WhereIf(!string.IsNullOrWhiteSpace(insuranceNo), e => e.InsuranceNo!.Contains(insuranceNo!))
            .WhereIf(discountGroup.HasValue, e => e.DiscountGroup != null && e.DiscountGroup == discountGroup)
            .WhereIf(gender.HasValue, e => e.Gender == gender)
            .WhereIf(isDeleted == true, e => e.IsDeleted) // Sadece isDeleted true ise filtre uygula
            .WhereIf(patientNumber.HasValue, e => e.PatientNumber == patientNumber);
    }
}