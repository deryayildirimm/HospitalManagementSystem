using System;
using Pusula.Training.HealthCare.Patients;

namespace Pusula.Training.HealthCare.Doctors;

public class DoctorWithDetails
{
    public virtual Guid Id { get; set; }
    public virtual string FirstName { get; set; } = string.Empty;
    public virtual string LastName { get; set; } = string.Empty;
    public virtual EnumGender Gender { get; set; }
    public virtual Guid DepartmentId { get; set; }
    public virtual string DepartmentName { get; set; } = string.Empty;
    public virtual Guid TitleId { get; set; }
    public virtual string TitleName { get; set; } = string.Empty;
}