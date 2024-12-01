using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.Treatment.Icds;

public interface IIcdRepository : IRepository<Icd, Guid>
{
    Task DeleteAllAsync(
        string? filterText = null,
        string? codeNumber = null,
        string? detail = null,
        CancellationToken cancellationToken = default);

    Task<List<Icd>> GetListAsync(
        string? filterText = null,
        string? codeNumber = null,
        string? detail = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string? filterText = null,
        string? codeNumber = null,
        string? detail = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);
}