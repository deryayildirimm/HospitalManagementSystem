using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.Restrictions;

public interface IRestrictionRepository : IRepository<Restriction, Guid>
{
    
    Task<List<Restriction>> GetListAsync(
        Guid? medicalServiceId,
        Guid? departmentId = null,
        Guid? doctorId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    );

    Task<long> GetCountAsync(
        Guid? medicalServiceId,
        Guid? departmentId = null,
        Guid? doctorId = null,
        CancellationToken cancellationToken = default);
}