using System;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.MedicalServices;

public class GetRestrictionsInput : PagedAndSortedResultRequestDto
{
    public Guid? MedicalServiceId { get; set; }
    public Guid? DoctorId { get; set; }
    public Guid? DepartmentId { get; set; }
    public EnumGender? Gender { get; set; }

    public GetRestrictionsInput()
    {
    }
}