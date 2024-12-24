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
    public string DoctorNameLastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string IdentityNumber { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public EnumGender Gender { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Now;
    public Guid CityId { get; set; }
    public CityDto City { get; set; } = null!;
    public Guid DistrictId { get; set; }
    public DistrictDto District { get; set; } = null!;
    public Guid DepartmentId { get; set; }
    public DepartmentDto Department { get; set; } = null!;
    public Guid TitleId { get; set; }
    public TitleDto Title { get; set; } = null!;
}