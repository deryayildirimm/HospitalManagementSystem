using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public interface IExaminationRepository : IRepository<Examination, Guid>
{
    Task DeleteAllAsync(
        string? filterText = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        string? complaint = null,
        string? story = null,
        Guid? protocolId = null,
        CancellationToken cancellationToken = default);

    Task<List<Examination>> GetListAsync(
        string? filterText = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        string? complaint = null,
        string? story = null,
        Guid? protocolId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string? filterText = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        string? complaint = null,
        string? story = null,
        Guid? protocolId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);
}