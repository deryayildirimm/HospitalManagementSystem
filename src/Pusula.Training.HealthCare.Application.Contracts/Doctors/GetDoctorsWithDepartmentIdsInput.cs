using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Doctors;

public class GetDoctorsWithDepartmentIdsInput : PagedAndSortedResultRequestDto
{
    public List<Guid>? DepartmentIds { get; set; }

    public string? Name { get; set; } = string.Empty;

}