using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Doctors;
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

    Task<MedicalService?> GetWithDetailsAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default
    );

    Task<List<MedicalService>> GetListAsync(
        Guid? departmentId = null,
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

    Task<List<MedicalService>> GetMedicalServiceListByDepartmentIdAsync(
        Guid departmentId,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<MedicalServiceWithDoctors> GetMedicalServiceWithDoctorsAsync(
        Guid medicalServiceId,
        Guid? departmentId,
        string? name = null,
        double? costMin = null,
        double? costMax = null,
        DateTime? serviceDateMin = null,
        DateTime? serviceDateMax = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<List<DoctorWithDetails>> GetMedicalServiceDoctorsAsync(
        Guid medicalServiceId,
        Guid departmentId,
        string? doctorFilterText = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        Guid? departmentId = null,
        string? name = null,
        double? costMin = null,
        double? costMax = null,
        DateTime? serviceDateMin = null,
        DateTime? serviceDateMax = null,
        CancellationToken cancellationToken = default
    );
}