using System;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Restrictions;

public class RestrictionExcelDownloadDto : PagedAndSortedResultRequestDto
{
    public string DownloadToken { get; set; } = null!;
    public Guid? MedicalServiceId { get; set; }
    public Guid? DoctorId { get; set; }
    public Guid? DepartmentId { get; set; }
    public EnumGender? Gender { get; set; }

    public RestrictionExcelDownloadDto()
    {
    }
}