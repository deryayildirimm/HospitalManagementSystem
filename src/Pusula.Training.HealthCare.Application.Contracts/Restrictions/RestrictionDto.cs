using System;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Restrictions;

public class RestrictionDto : FullAuditedEntityDto<Guid>
{
    public virtual MedicalServiceDto MedicalService { get; protected set; } = null!;
    public virtual Guid MedicalServiceId { get; protected set; }
    public virtual DepartmentDto Department { get; protected set; } = null!;
    public virtual Guid DepartmentId { get; protected set; }
    public virtual DoctorDto Doctor { get; protected set; } = null!;
    public virtual Guid DoctorId { get; protected set; }
    public virtual int? MinAge { get; protected set; }
    public virtual int? MaxAge { get; protected set; }
    public virtual EnumGender? AllowedGender { get; protected set; }
}