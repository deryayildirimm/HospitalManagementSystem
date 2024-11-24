using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.Patients;

public interface IPatientRepository : IRepository<Patient, Guid>
{
    Task DeleteAllAsync(
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
        CancellationToken cancellationToken = default);

    Task<List<Patient>> GetListAsync(
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
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
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
        CancellationToken cancellationToken = default);
    
    Task<Patient> GetPatientByNumberAsync(
        int patientNumber ,
        CancellationToken cancellationToken = default
    );
}