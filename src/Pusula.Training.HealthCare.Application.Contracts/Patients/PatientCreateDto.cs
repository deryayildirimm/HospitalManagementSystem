using JetBrains.Annotations;
using Pusula.Training.HealthCare.Countries;
using Pusula.Training.HealthCare.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Patients;

public class PatientCreateDto
{
    public bool identityField { get; set; } = false;
    public bool passportField { get; set; } = false;

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
    public string IdentityNumber { get; set; } = null!;

    [Required]
    public string Nationality { get; set; } = null!;

    [PassportNumberValidator]
    public string? PassportNumber { get; set; } = null!;

    [Required]
    public DateTime BirthDate { get; set; }

    [EmailAddress]
    [StringLength(PatientConsts.EmailAddressMaxLength)]
    public string EmailAddress { get; set; } = null!;

    [Required]
    public string MobilePhoneNumber { get; set; } = null!;

    [Range(PatientConsts.RelativeMinValue, PatientConsts.RelativeMaxValue)]
    public EnumRelative Relative { get; set; }

    public string? RelativePhoneNumber { get; set; }

    [Required]
    [Range(PatientConsts.PatientTypeMinValue, PatientConsts.PatientTypeMaxValue)]
    public EnumPatientTypes PatientType { get; set; }

    [StringLength(PatientConsts.AddressMaxLength)]
    public string? Address { get; set; }

    [Required]
    [Range(PatientConsts.InsuranceMinValue, PatientConsts.InsuranceMaxValue)]
    public EnumInsuranceType InsuranceType { get; set; }

    [Required]
    [InsuranceNumberValidator]
    public string InsuranceNo { get; set; } = null!;

    [Range(PatientConsts.DiscountGroupMinValue, PatientConsts.DiscountGroupMaxValue)]
    public EnumDiscountGroup DiscountGroup { get; set; }

    [Required]
    [Range(PatientConsts.GenderMinValue, PatientConsts.GenderMaxValue)]
    public EnumGender Gender { get; set; }

}