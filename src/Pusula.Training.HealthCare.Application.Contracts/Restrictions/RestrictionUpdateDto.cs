using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Validators;

namespace Pusula.Training.HealthCare.Restrictions;

public class RestrictionUpdateDto
{
    [Required]
    [GuidValidator]
    public virtual Guid MedicalServiceId { get; protected set; }
    
    [Required]
    [GuidValidator]
    public virtual Guid DepartmentId { get; protected set; }
    
    [GuidValidator]
    public virtual Guid DoctorId { get; protected set; }
    
    [Range(RestrictionConsts.AgeMinValue, RestrictionConsts.AgeMaxValue)]
    public virtual int? MinAge { get; protected set; }
    
    [Range(RestrictionConsts.AgeMinValue, RestrictionConsts.AgeMaxValue)]
    public virtual int? MaxAge { get; protected set; }
    
    public virtual EnumGender? AllowedGender { get; protected set; }
}