using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;

public interface IBackgroundRepository : IRepository<Background, Guid>
{
    Task DeleteAllAsync(
        string? filterText = null,
        string? allergies = null,
        string? medications = null,
        string? habits = null,
        Guid? examinationId = null,
        CancellationToken cancellationToken = default);

    Task<List<Background>> GetListAsync(
        string? filterText = null,
        string? allergies = null,
        string? medications = null,
        string? habits = null,
        Guid? examinationId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string? filterText = null,
        string? allergies = null,
        string? medications = null,
        string? habits = null,
        Guid? examinationId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);
}