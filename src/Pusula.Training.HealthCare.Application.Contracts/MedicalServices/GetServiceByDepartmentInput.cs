using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.MedicalServices;

public class GetServiceByDepartmentInput : PagedAndSortedResultRequestDto
{
    public Guid DepartmentId { get; set; }
    
    public GetServiceByDepartmentInput()
    {
    }
}