using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.MedicalServices;

public class GetDepartmentServiceDoctorsInput : PagedAndSortedResultRequestDto
{
    public Guid MedicalServiceId { get; set; }
    public Guid DepartmentId { get; set; }
    public string? DoctorFilterText { get; set; }

    public GetDepartmentServiceDoctorsInput()
    {
    }
}