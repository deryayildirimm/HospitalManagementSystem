using System;
using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Districts;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Titles;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Doctors;

public class DoctorDto : FullAuditedEntityDto<Guid>
{
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
    public CityDto City { get; set; } = null!;
    [Required]
    public Guid DistrictId { get; set; }
    public DistrictDto District { get; set; } = null!;
    [Required]
    public Guid DepartmentId { get; set; }
    public DepartmentDto Department { get; set; } = null!;
    [Required]
    public Guid TitleId { get; set; }
    public TitleDto Title { get; set; } = null!;
}