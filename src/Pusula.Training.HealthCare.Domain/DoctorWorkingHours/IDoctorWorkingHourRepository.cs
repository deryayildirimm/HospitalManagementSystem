using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.DoctorWorkingHours;

public interface IDoctorWorkingHourRepository : IRepository<DoctorWorkingHour, Guid>
{
    Task<List<DoctorWorkingHour>> GetListAsync(
        Guid? doctorId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    );
}