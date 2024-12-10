using System;
using Pusula.Training.HealthCare.Shared;

namespace Pusula.Training.HealthCare.Doctors;

public class DoctorLookupDto<TKey> : BaseLookupDto<TKey>
{
    public Guid DepartmentId { get; set; }
}