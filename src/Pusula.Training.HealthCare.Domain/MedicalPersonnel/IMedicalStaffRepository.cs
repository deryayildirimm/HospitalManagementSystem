using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.MedicalPersonnel;

public interface IMedicalStaffRepository : IRepository<MedicalStaff, Guid>
{
    Task DeleteAllAsync(
        string? filterText = null,
        string? firstName = null,
        string? lastName = null,
        string? identityNumber = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        EnumGender? gender = null,
        string? email = null,
        string? phoneNumber = null,
        int? yearOfExperienceMin = 0,
        Guid? cityId = null,
        Guid? districtId = null,
        Guid? departmentId = null,
        CancellationToken cancellationToken = default);

    Task<MedicalStaffWithNavigationProperties> GetWithNavigationPropertiesAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<List<MedicalStaffWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
        string? filterText = null,
        string? firstName = null,
        string? lastName = null,
        string? identityNumber = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        EnumGender? gender = null,
        string? email = null,
        string? phoneNumber = null,
        int? yearOfExperienceMin = 0,
        Guid? cityId = null,
        Guid? districtId = null,
        Guid? departmentId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    );
    Task<List<MedicalStaff>> GetListAsync(
        string? filterText = null,
        string? firstName = null,
        string? lastName = null,
        string? identityNumber = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        EnumGender? gender = null,
        string? email = null,
        string? phoneNumber = null,
        int? yearOfExperienceMin = 0,
        Guid? cityId = null,
        Guid? districtId = null,
        Guid? departmentId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string? filterText = null,
        string? firstName = null,
        string? lastName = null,
        string? identityNumber = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        EnumGender? gender = null,
        string? email = null,
        string? phoneNumber = null,
        int? yearOfExperienceMin = 0,
        Guid? cityId = null,
        Guid? districtId = null,
        Guid? departmentId = null,
        CancellationToken cancellationToken = default);
}