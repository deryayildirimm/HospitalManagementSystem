using JetBrains.Annotations;
using Pusula.Training.HealthCare.Validators;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.Patients;

public class PatientUpdateDto : IHasConcurrencyStamp
{

    [Required]
    [StringLength(PatientConsts.NameMaxLength)]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(PatientConsts.LastNameMaxLength)]
    public string LastName { get; set; } = null!;

    [StringLength(PatientConsts.NameMaxLength)]
    public string? MothersName { get; set; }

    [StringLength(PatientConsts.NameMaxLength)]
    public string? FathersName { get; set; }

    [IdentityNumberValidator]
    public string IdentityAndPassportNumber { get; set; } = null!;

    public string? Nationality { get; set; }

    [Required]
    public DateTime BirthDate { get; set; }

    [EmailAddress]
    [StringLength(PatientConsts.EmailAddressMaxLength)]
    public string? EmailAddress { get; set; }

    [UpdatePhoneNumberValidator]
    public string? MobilePhoneNumber { get; set; }

    [Range(PatientConsts.RelativeMinValue, PatientConsts.RelativeMaxValue)]
    public EnumRelative Relative { get; set; }

    [UpdatePhoneNumberValidator]
    public string? RelativePhoneNumber { get; set; }

    [Range(PatientConsts.PatientTypeMinValue, PatientConsts.PatientTypeMaxValue)]
    public EnumPatientTypes PatientType { get; set; }

    [StringLength(PatientConsts.AddressMaxLength)]
    public string? Address { get; set; }

    [Range(PatientConsts.DiscountGroupMinValue, PatientConsts.DiscountGroupMaxValue)]
    public EnumDiscountGroup DiscountGroup { get; set; }

    [Required]
    [Range(PatientConsts.GenderMinValue, PatientConsts.GenderMaxValue)]
    public EnumGender Gender { get; set; }

    public bool IsDeleted { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}