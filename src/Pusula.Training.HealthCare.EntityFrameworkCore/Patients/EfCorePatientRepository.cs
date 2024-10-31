using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.Patients;

public class EfCorePatientRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, Patient, Guid>(dbContextProvider), IPatientRepository
{
    public virtual async Task DeleteAllAsync(
        string? filterText = null,
        string? firstName = null,
        string? lastName = null,
        string? identityNumber = null,
        EnumNationality? nationality = null,
        string? passportNumber = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        string? emailAddress = null,
        string? mobilePhoneNumber = null,
        string? homePhoneNumber = null,
        EnumPatientTypes? patientType = null,
        EnumInsuranceType? insuranceType = null,
        string? insuranceNo = null,
        EnumDiscountGroup? discountGroup = null,
        EnumGender? gender = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        query = ApplyFilter(query, filterText, firstName, lastName, identityNumber, nationality, passportNumber, birthDateMin, birthDateMax, 
            emailAddress, mobilePhoneNumber, homePhoneNumber, patientType, insuranceType, insuranceNo, discountGroup, gender);

        var ids = query.Select(x => x.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<Patient>> GetListAsync(
        string? filterText = null,
        string? firstName = null,
        string? lastName = null,
        string? identityNumber = null,
        EnumNationality? nationality = null,
        string? passportNumber = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        string? emailAddress = null,
        string? mobilePhoneNumber = null,
        string? homePhoneNumber = null,
        EnumPatientTypes? patientType = null,
        EnumInsuranceType? insuranceType = null,
        string? insuranceNo = null,
        EnumDiscountGroup? discountGroup = null,
        EnumGender? gender = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, firstName, lastName, identityNumber, nationality, passportNumber, birthDateMin, 
            birthDateMax, emailAddress, mobilePhoneNumber, homePhoneNumber, patientType, insuranceType, insuranceNo, discountGroup, gender);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PatientConsts.GetDefaultSorting(false) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? filterText = null,
        string? firstName = null,
        string? lastName = null,
        string? identityNumber = null,
        EnumNationality? nationality = null,
        string? passportNumber = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        string? emailAddress = null,
        string? mobilePhoneNumber = null,
        string? homePhoneNumber = null,
        EnumPatientTypes? patientType = null,
        EnumInsuranceType? insuranceType = null,
        string? insuranceNo = null,
        EnumDiscountGroup? discountGroup = null,
        EnumGender? gender = null,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetDbSetAsync()), filterText, firstName, lastName, identityNumber, nationality, passportNumber, birthDateMin, birthDateMax, 
            emailAddress, mobilePhoneNumber, homePhoneNumber, patientType, insuranceType, insuranceNo, discountGroup, gender);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<Patient> ApplyFilter(
        IQueryable<Patient> query,
        string? filterText = null,
        string? firstName = null,
        string? lastName = null,
        string? identityNumber = null,
        EnumNationality? nationality = null,
        string? passportNumber = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        string? emailAddress = null,
        string? mobilePhoneNumber = null,
        string? homePhoneNumber = null,
        EnumPatientTypes? patientType = null,
        EnumInsuranceType? insuranceType = null,
        string? insuranceNo = null,
        EnumDiscountGroup? discountGroup = null,
        EnumGender? gender = null)
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filterText),
                e => e.FirstName!.Contains(filterText!) || e.LastName!.Contains(filterText!) ||
                     e.IdentityNumber!.Contains(filterText!) || e.EmailAddress!.Contains(filterText!) ||
                     e.MobilePhoneNumber!.Contains(filterText!) || e.HomePhoneNumber!.Contains(filterText!))
            .WhereIf(!string.IsNullOrWhiteSpace(firstName), e => e.FirstName.ToLower().Contains(firstName!.ToLower()))
            .WhereIf(!string.IsNullOrWhiteSpace(lastName), e => e.LastName.ToLower().Contains(lastName!.ToLower()))
            .WhereIf(!string.IsNullOrWhiteSpace(identityNumber), e => e.IdentityNumber!.Contains(identityNumber!))
            .WhereIf(nationality.HasValue, e => e.Nationality == nationality)
            .WhereIf(!string.IsNullOrWhiteSpace(passportNumber), e => e.PassportNumber!.ToLower().Contains(passportNumber!.ToLower()))
            .WhereIf(birthDateMin.HasValue, e => e.BirthDate >= birthDateMin!.Value)
            .WhereIf(birthDateMax.HasValue, e => e.BirthDate <= birthDateMax!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(emailAddress), e => e.EmailAddress!.Contains(emailAddress!))
            .WhereIf(!string.IsNullOrWhiteSpace(mobilePhoneNumber), e => e.MobilePhoneNumber.Contains(mobilePhoneNumber!))
            .WhereIf(!string.IsNullOrWhiteSpace(homePhoneNumber), e => e.HomePhoneNumber != null && e.HomePhoneNumber.Contains(homePhoneNumber!))
            .WhereIf(patientType.HasValue, e => e.PatientType == patientType)
            .WhereIf(insuranceType.HasValue, e => e.InsuranceType == insuranceType)
            .WhereIf(!string.IsNullOrWhiteSpace(insuranceNo), e => e.InsuranceNo!.Contains(insuranceNo!))
            .WhereIf(discountGroup.HasValue, e => e.DiscountGroup != null && e.DiscountGroup == discountGroup)
            .WhereIf(gender.HasValue, e => e.Gender == gender);
    }
}