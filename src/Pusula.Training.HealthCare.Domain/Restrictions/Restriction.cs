using System;
using JetBrains.Annotations;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.GlobalExceptions;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Restrictions;

public class Restriction : FullAuditedAggregateRoot<Guid>
{
    [NotNull] public virtual MedicalService MedicalService { get; private set; } = null!;
    [NotNull] public virtual Guid MedicalServiceId { get; private set; }
    [NotNull] public virtual Department Department { get; private set; } = null!;
    [NotNull] public virtual Guid DepartmentId { get; private set; }
    [CanBeNull] public virtual Doctor? Doctor { get; private set; }
    [CanBeNull] public virtual Guid? DoctorId { get; private set; }
    [CanBeNull] public virtual int? MinAge { get; private set; }
    [CanBeNull] public virtual int? MaxAge { get; private set; }
    [NotNull] public virtual EnumGender AllowedGender { get; private set; }

    protected Restriction()
    {
        DoctorId = Guid.Empty;
        DepartmentId = Guid.Empty;
        MedicalServiceId = Guid.Empty;
    }

    public Restriction(Guid id, Guid medicalServiceId, Guid departmentId, Guid? doctorId,
        int? minAge, int? maxAge, EnumGender allowedGender)
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

    public void SetDoctorId(Guid? doctorId = null)
    {
        DoctorId = doctorId;
    }

    public void SetMinAge(int? minAge = null)
    {
        HealthCareGlobalException.ThrowIf(
            HealthCareDomainErrorKeyValuePairs.ValueExceedLimit,
            minAge is > RestrictionConsts.AgeMaxValue or < RestrictionConsts.AgeMinValue
        );
        MinAge = minAge;
    }

    public void SetMaxAge(int? maxAge = null)
    {
        HealthCareGlobalException.ThrowIf(
            HealthCareDomainErrorKeyValuePairs.ValueExceedLimit,
            maxAge is > RestrictionConsts.AgeMaxValue or < RestrictionConsts.AgeMinValue
        );
        MaxAge = maxAge;
    }

    public void SetAllowedGender(EnumGender allowedGender)
    {
        HealthCareGlobalException.ThrowIf(
            HealthCareDomainErrorKeyValuePairs.GenderNotValid,
            ((int)allowedGender > RestrictionConsts.GenderMaxValue ||
             (int)allowedGender < RestrictionConsts.GenderMinValue)
        );
        AllowedGender = allowedGender;
    }
}