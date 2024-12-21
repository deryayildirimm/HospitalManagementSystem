using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Treatment.Icds;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public interface IExaminationRepository : IRepository<Examination, Guid>
{
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

    Task<long> GetIcdReportCountAsync(
        DateTime startDate,
        DateTime? endDate = null,
        string? filterText = null,
        string? codeNumber = null,
        string? detail = null,
        CancellationToken cancellationToken = default);
    
    Task<List<IcdReport>> GetIcdReportAsync(
        DateTime startDate,
        DateTime? endDate = null,
        string? filterText = null,
        string? codeNumber = null,
        string? detail = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<Examination?> GetWithNavigationPropertiesAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Examination?> GetByProtocolIdAsync(
        Guid? protocolId,
        CancellationToken cancellationToken = default);


    Task UpdateExaminationIcdsAsync(Guid examinationId, List<Guid> icdIds,
        CancellationToken cancellationToken = default);
}