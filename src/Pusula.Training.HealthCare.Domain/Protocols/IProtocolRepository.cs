using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.Protocols;

public interface IProtocolRepository : IRepository<Protocol, Guid>
{
    Task DeleteAllAsync(
        string? filterText = null,
        string? type = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        string? endTime = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        CancellationToken cancellationToken = default);
    Task<ProtocolWithNavigationProperties> GetWithNavigationPropertiesAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<List<ProtocolWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
        string? filterText = null,
        string? type = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        string? endTime = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    );

    Task<List<Protocol>> GetListAsync(
                string? filterText = null,
                string? type = null,
                DateTime? startTimeMin = null,
                DateTime? startTimeMax = null,
                string? endTime = null,
                string? sorting = null,
                int maxResultCount = int.MaxValue,
                int skipCount = 0,
                CancellationToken cancellationToken = default
            );

    Task<long> GetCountAsync(
        string? filterText = null,
        string? type = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        string? endTime = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        CancellationToken cancellationToken = default);
}