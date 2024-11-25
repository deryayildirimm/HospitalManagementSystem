using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public interface IDoctorLeaveManager
{
    Task<DoctorLeave> CreateAsync(Guid doctorId, DateTime startDate, DateTime endDate, string? reason = null);

    Task<DoctorLeave> UpdateAsync(Guid id, Guid doctorId,
        DateTime startDate, DateTime endDate, string? reason = null, [CanBeNull] string? concurrencyStamp = null);
}