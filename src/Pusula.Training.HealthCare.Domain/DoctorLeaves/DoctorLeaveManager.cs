using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Pusula.Training.HealthCare.GlobalExceptions;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public class DoctorLeaveManager(IDoctorLeaveRepository doctorLeaveRepository) : DomainService, IDoctorLeaveManager
{
    public virtual async Task<DoctorLeave> CreateAsync(Guid doctorId, DateTime startDate, DateTime endDate,
        EnumLeaveType enumLeaveType, string? reason = null)
    {
        Check.NotNull(doctorId, nameof(doctorId));
        Check.NotNull(startDate, nameof(startDate));
        Check.NotNull(endDate, nameof(endDate));
        Check.NotNull(enumLeaveType, nameof(enumLeaveType));

        HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.DoctorRequired,
            doctorId == Guid.Empty);

        //Check if doctor already has a leave between selected dates
        HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.DoctorAlreadyHasLeave,
            await doctorLeaveRepository.FirstOrDefaultAsync(x =>
                x.DoctorId == doctorId &&
                x.StartDate >= startDate && x.EndDate <= endDate) is not null);

        HealthCareGlobalException.ThrowIf(HealthCareDomainErrorCodes.InvalidDateRange_MESSAGE,
            HealthCareDomainErrorCodes.InvalidDateRange_CODE,
            startDate > endDate);

        var leaves = new DoctorLeave(
            GuidGenerator.Create(),
            doctorId,
            startDate,
            endDate,
            enumLeaveType,
            reason);

        return await doctorLeaveRepository.InsertAsync(leaves);
    }

    public virtual async Task<DoctorLeave> UpdateAsync(Guid id, Guid doctorId,
        DateTime startDate, DateTime endDate, EnumLeaveType enumLeaveType, string? reason = null,
        [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNull(doctorId, nameof(doctorId));
        Check.NotNull(startDate, nameof(startDate));
        Check.NotNull(endDate, nameof(endDate));
        Check.NotNull(enumLeaveType, nameof(enumLeaveType));

        HealthCareGlobalException.ThrowIf(HealthCareDomainErrorCodes.InvalidDateRange_MESSAGE,
            HealthCareDomainErrorCodes.InvalidDateRange_CODE,
            startDate > endDate);

        var leaves = await doctorLeaveRepository.GetAsync(id);

        leaves.SetDoctorId(doctorId);
        leaves.SetStartDate(startDate);
        leaves.SetEndDate(endDate);
        leaves.SetReason(reason);
        leaves.SetType(enumLeaveType);

        leaves.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await doctorLeaveRepository.UpdateAsync(leaves);
    }
}