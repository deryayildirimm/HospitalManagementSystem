using System;
using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.Patients;

namespace Pusula.Training.HealthCare.Doctors;

public class DoctorUpdateDto
{
    [Required]
    public Guid Id { get; set; } = default!;
    [Required]
    [StringLength(DoctorConsts.FirstNameMaxLength, MinimumLength = DoctorConsts.FirstNameMinLength)]
    public string FirstName { get; set; } = null!;
    [Required]
    [StringLength(DoctorConsts.LastNameMaxLength, MinimumLength = DoctorConsts.LastNameMinLength)]
    public string LastName { get; set; } = null!;
    [Required]
    [StringLength(DoctorConsts.IdentityNumberLength, MinimumLength = DoctorConsts.IdentityNumberLength)]
    public string IdentityNumber { get; set; } = null!;
    [Required]
    public DateTime BirthDate { get; set; }
    [Required]
    public EnumGender Gender { get; set; }
    [StringLength(DoctorConsts.EmailMaxLength, MinimumLength = DoctorConsts.EmailMinLength)]
    public string? Email { get; set; }
    [StringLength(DoctorConsts.PhoneNumberMaxLength)]
    public string? PhoneNumber { get; set; }
    [Required] 
    public DateTime StartDate { get; set; } = DateTime.Now;
    [Required]
    public Guid CityId { get; set; }
    [Required]
    public Guid DistrictId { get; set; }
    [Required]
    public Guid DepartmentId { get; set; }
    [Required]
    public Guid TitleId { get; set; }
}