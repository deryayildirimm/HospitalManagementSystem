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
        string? note = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        DateTime? endTimeMin = null,
        DateTime? endTimeMax = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        Guid? doctorId = null,
        Guid? insuranceId = null,
        CancellationToken cancellationToken = default);
    Task<Protocol> GetWithNavigationPropertiesAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );
    
    Task<Protocol> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );


    Task<List<ProtocolWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
        string? filterText = null,
        string? note = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        DateTime? endTimeMin = null,
        DateTime? endTimeMax = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        Guid? doctorId = null,
        Guid? insuranceId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    );

    Task<List<Protocol>> GetListAsync(
                string? filterText = null,
                string? note = null,
                DateTime? startTimeMin = null,
                DateTime? startTimeMax = null,
                DateTime? endTimeMin = null,
                DateTime? endTimeMax = null,
                string? sorting = null,
                int maxResultCount = int.MaxValue,
                int skipCount = 0,
                CancellationToken cancellationToken = default
            );

    Task<long> GetCountAsync(
        string? filterText = null,
        string? note = null,
        DateTime? startTimeMin = null,
        DateTime? startTimeMax = null,
        DateTime? endTimeMin = null,
        DateTime? endTimeMax = null,
        Guid? patientId = null,
        Guid? departmentId = null,
        Guid? protocolTypeId = null,
        Guid? doctorId = null,
        Guid? insuranceId = null,
        CancellationToken cancellationToken = default);
}