using Volo.Abp.Application.Dtos;
using System;

namespace Pusula.Training.HealthCare.Patients;

public class GetPatientsInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public int? PatientNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? IdentityAndPassportNumber { get; set; }
    public string? Nationality { get; set; }
    public DateTime? BirthDateMin { get; set; }
    public DateTime? BirthDateMax { get; set; }
    public string? EmailAddress { get; set; }
    public string? MobilePhoneNumber { get; set; }
    public EnumPatientTypes? PatientType { get; set; }
    public EnumDiscountGroup? DiscountGroup { get; set; }
    public EnumGender? Gender { get; set; }

    public bool? IsDeleted { get; set; }

    public GetPatientsInput()
    {
    }
}