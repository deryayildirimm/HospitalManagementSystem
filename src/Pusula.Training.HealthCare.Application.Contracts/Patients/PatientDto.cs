using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.Patients;

public class PatientDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public int PatientNumber { get; set; } = 0;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MothersName { get; set; }
    public string? FathersName { get; set; }
    public string? IdentityNumber { get; set; }
    public string Nationality { get; set; } = null!;
    public string? PassportNumber { get; set; }
    public DateTime BirthDate { get; set; }
    public string? EmailAddress { get; set; }
    public string MobilePhoneNumber { get; set; } = null!;
    public EnumRelative? Relative { get; set; }
    public string? RelativePhoneNumber { get; set; }
    public EnumPatientTypes PatientType { get; set; }
    public string? Address { get; set; }
    public EnumInsuranceType InsuranceType { get; set; }
    public string? InsuranceNo { get; set; }
    public EnumDiscountGroup? DiscountGroup { get; set; }
    public EnumGender Gender { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;

}