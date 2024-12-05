using JetBrains.Annotations;
using Pusula.Training.HealthCare.Countries;
using Pusula.Training.HealthCare.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Patients;

public class PatientCreateDto
{

    [Required]
    [StringLength(PatientConsts.NameMaxLength,MinimumLength = PatientConsts.NameMinLength)]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(PatientConsts.LastNameMaxLength, MinimumLength =PatientConsts.LastNameMinLength)]
    public string LastName { get; set; } = null!;

    [StringLength(PatientConsts.NameMaxLength, MinimumLength = PatientConsts.NameMinLength)]
    public string? MothersName { get; set; }

    [StringLength(PatientConsts.NameMaxLength, MinimumLength = PatientConsts.NameMinLength)]
    public string? FathersName { get; set; }

    [IdentityNumberValidator]
    public string IdentityAndPassportNumber { get; set; } = null!;

    public string? Nationality { get; set; } 

    [Required]
    public DateTime BirthDate { get; set; }

    [EmailAddress]
    [StringLength(PatientConsts.EmailAddressMaxLength)]
    public string? EmailAddress { get; set; }

    public string? MobilePhoneNumber { get; set; } 

    [Range(PatientConsts.RelativeMinValue, PatientConsts.RelativeMaxValue)]
    public EnumRelative Relative { get; set; }

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

}