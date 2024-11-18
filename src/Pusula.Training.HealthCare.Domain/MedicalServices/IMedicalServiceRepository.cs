using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.MedicalServices;

public interface IMedicalServiceRepository : IRepository<MedicalService, Guid>
{
    Task DeleteAllAsync(
        string? name = null,
        double? costMin = null,
        double? costMax = null,
        DateTime? serviceDateMin = null,
        DateTime? serviceDateMax = null,
        CancellationToken cancellationToken = default
    );

    Task<List<MedicalService>> GetListAsync(
        string? name = null,
        double? costMin = null,
        double? costMax = null,
        DateTime? serviceDateMin = null,
        DateTime? serviceDateMax = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    );
    
    Task<List<MedicalServiceWithDepartments>> GetMedicalServiceWithDepartmentsAsync(
        string? name = null,
        double? costMin = null,
        double? costMax = null,
        DateTime? serviceDateMin = null,
        DateTime? serviceDateMax = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    );

    Task<long> GetCountAsync(
        string? name = null,
        double? costMin = null,
        double? costMax = null,
        DateTime? serviceDateMin = null,
        DateTime? serviceDateMax = null,
        CancellationToken cancellationToken = default
    );
    
}