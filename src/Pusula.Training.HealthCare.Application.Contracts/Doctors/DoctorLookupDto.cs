using System;
using Pusula.Training.HealthCare.Shared;

namespace Pusula.Training.HealthCare.Doctors;

public class DoctorLookupDto : BaseLookupDto<Guid>
{
    public Guid DepartmentId { get; set; }
}