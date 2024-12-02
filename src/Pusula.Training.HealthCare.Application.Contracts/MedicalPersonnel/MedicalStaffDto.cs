using System;
using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Districts;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.MedicalPersonnel;

public class MedicalStaffDto : FullAuditedEntityDto<Guid>
{
    [Required]
    [StringLength(MedicalStaffConsts.FirstNameMaxLength, MinimumLength = MedicalStaffConsts.FirstNameMinLength)]
    public string FirstName { get; set; } = null!;
    [Required]
    [StringLength(MedicalStaffConsts.LastNameMaxLength, MinimumLength = MedicalStaffConsts.LastNameMinLength)]
    public string LastName { get; set; } = null!;
    [Required]
    [StringLength(MedicalStaffConsts.IdentityNumberLength, MinimumLength = MedicalStaffConsts.IdentityNumberLength)]
    public string IdentityNumber { get; set; } = null!;
    [Required]
    public DateTime BirthDate { get; set; }
    [Required]
    public EnumGender Gender { get; set; }
    [StringLength(MedicalStaffConsts.EmailMaxLength, MinimumLength = MedicalStaffConsts.EmailMinLength)]
    public string? Email { get; set; }
    [StringLength(MedicalStaffConsts.PhoneNumberMaxLength)]
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
}