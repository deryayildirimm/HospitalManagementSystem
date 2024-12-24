using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.Treatment.Examinations.PhysicalFindings;

public interface IPhysicalFindingRepository : IRepository<PhysicalFinding, Guid>
{

    Task<List<PhysicalFinding>> GetListAsync(
        int? weightMin = null,
        int? weightMax = null,
        int? heightMin = null,
        int? heightMax = null,
        Guid? examinationId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        int? weightMin = null,
        int? weightMax = null,
        int? heightMin = null,
        int? heightMax = null,
        Guid? examinationId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);
}