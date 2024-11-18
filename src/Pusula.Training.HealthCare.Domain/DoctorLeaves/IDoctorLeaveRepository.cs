using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public interface IDoctorLeaveRepository : IRepository<DoctorLeave, Guid>
{
        Task DeleteAllAsync(
        string? filterText = null,
        Guid? doctorId = null,
        DateTime? startDateMin = null,
        DateTime? startDateMax = null,
        DateTime? endDateMin = null,
        DateTime? endDateMax = null,
        string? reason = null,
        CancellationToken cancellationToken = default);

    Task<List<DoctorLeave>> GetListAsync(
        string? filterText = null,
        Guid? doctorId = null,
        DateTime? startDateMin = null,
        DateTime? startDateMax = null,
        DateTime? endDateMin = null,
        DateTime? endDateMax = null,
        string? reason = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string? filterText = null,
        Guid? doctorId = null,
        DateTime? startDateMin = null,
        DateTime? startDateMax = null,
        DateTime? endDateMin = null,
        DateTime? endDateMax = null,
        string? reason = null,
        CancellationToken cancellationToken = default);

    Task<List<DoctorLeave>> GetListByDoctorNumberAsync(
        Guid? doctorId,
        string? identityNumber, 
        CancellationToken cancellationToken = default);
}