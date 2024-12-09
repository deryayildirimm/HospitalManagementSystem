using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;

public interface IFamilyHistoryRepository : IRepository<FamilyHistory, Guid>
{
    Task DeleteAllAsync(
        string? filterText = null,
        string? motherDisease = null,
        string? fatherDisease = null,
        string? sisterDisease = null,
        string? brotherDisease = null,
        bool? areParentsRelated = null,
        Guid? examinationId = null,
        CancellationToken cancellationToken = default);

    Task<List<FamilyHistory>> GetListAsync(
        string? filterText = null,
        string? motherDisease = null,
        string? fatherDisease = null,
        string? sisterDisease = null,
        string? brotherDisease = null,
        bool? areParentsRelated = null,
        Guid? examinationId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string? filterText = null,
        string? motherDisease = null,
        string? fatherDisease = null,
        string? sisterDisease = null,
        string? brotherDisease = null,
        bool? areParentsRelated = null,
        Guid? examinationId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);
}