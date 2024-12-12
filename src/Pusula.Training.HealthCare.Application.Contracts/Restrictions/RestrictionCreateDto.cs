using System;
using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Validators;

namespace Pusula.Training.HealthCare.Restrictions;

public class RestrictionCreateDto
{
    
    [Required]
    [GuidValidator]
    public virtual Guid MedicalServiceId { get; set; }
    
    [Required]
    [GuidValidator]
    public virtual Guid DepartmentId { get; set; }
    
    [GuidValidator]
    public virtual Guid DoctorId { get; set; }
    
    [Range(RestrictionConsts.AgeMinValue, RestrictionConsts.AgeMaxValue)]
    public virtual int? MinAge { get;  set; }
    
    [Range(RestrictionConsts.AgeMinValue, RestrictionConsts.AgeMaxValue)]
    public virtual int? MaxAge { get;  set; }
    
    public virtual EnumGender? AllowedGender { get;  set; }
}