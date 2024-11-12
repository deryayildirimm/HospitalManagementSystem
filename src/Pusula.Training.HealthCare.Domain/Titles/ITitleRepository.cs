using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.Titles;

public interface ITitleRepository : IRepository<Title, Guid>
{
    Task DeleteAllAsync(
        string? filterText = null,
        string? titleName = null,
        CancellationToken cancellationToken = default);

    Task<List<Title>> GetListAsync(
        string? filterText = null,
        string? titleName = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string? filterText = null,
        string? titleName = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);
}