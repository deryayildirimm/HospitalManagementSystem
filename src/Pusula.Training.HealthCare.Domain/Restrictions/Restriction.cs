using System;
using JetBrains.Annotations;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Restrictions;

public class Restriction : FullAuditedAggregateRoot<Guid>
{
    [NotNull] public virtual MedicalService MedicalService { get; protected set; }
    [NotNull] public virtual Guid MedicalServiceId { get; protected set; }
    
    [NotNull] public virtual Department Department { get; protected set; }
    [NotNull] public virtual Guid DepartmentId { get; protected set; }

    [CanBeNull] public virtual Doctor? Doctor { get; protected set; }
    [CanBeNull] public virtual Guid DoctorId { get; protected set; }
    [CanBeNull] public virtual int? MinAge { get; protected set; }
    [CanBeNull] public virtual int? MaxAge { get; protected set; }
    [CanBeNull] public virtual EnumGender? AllowedGender { get; protected set; }

    protected Restriction()
    {
        DoctorId = Guid.Empty;
        DepartmentId = Guid.Empty;
        MedicalServiceId = Guid.Empty;
    }

    public Restriction(Guid id, Guid medicalServiceId, Guid departmentId, Guid doctorId,
        int? minAge, int? maxAge, EnumGender? allowedGender)
    {
        Id = id;
        SetMedicalServiceId(medicalServiceId);
        SetDepartmentId(departmentId);
        SetDoctorId(doctorId);
        SetMinAge(minAge);
        SetMaxAge(maxAge);
        SetAllowedGender(allowedGender);
    }

    public void SetMedicalServiceId(Guid medicalServiceId)
    {
        Check.NotNull(medicalServiceId, nameof(medicalServiceId));
        MedicalServiceId = medicalServiceId;
    }

    public void SetDepartmentId(Guid departmentId)
    {
        Check.NotNull(departmentId, nameof(departmentId));
        DepartmentId = departmentId;
    }

    public void SetDoctorId(Guid doctorId)
    {
        Check.NotNull(doctorId, nameof(doctorId));
        DoctorId = doctorId;
    }

    public void SetMinAge(int? minAge)
    {
        Check.NotNull(minAge, nameof(minAge));
        Check.Range((int)minAge, nameof(minAge), RestrictionConsts.AgeMinValue, RestrictionConsts.AgeMaxValue);
        MinAge = minAge;
    }

    public void SetMaxAge(int? maxAge)
    {
        Check.NotNull(maxAge, nameof(maxAge));
        Check.Range((int)maxAge, nameof(maxAge), RestrictionConsts.AgeMinValue, RestrictionConsts.AgeMaxValue);
        MaxAge = maxAge;
    }

    public void SetAllowedGender(EnumGender? allowedGender)
    {
        Check.NotNull(allowedGender, nameof(allowedGender));
        Check.Range((int)allowedGender, nameof(allowedGender), RestrictionConsts.GenderMinValue,
            RestrictionConsts.GenderMaxValue);
        AllowedGender = allowedGender;
    }
}