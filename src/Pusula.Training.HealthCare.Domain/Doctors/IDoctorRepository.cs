using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.Doctors;

public interface IDoctorRepository : IRepository<Doctor, Guid>
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
        int? yearOfExperience = 0,
        string? city = null,
        string? district = null,
        Guid? titleId = null,
        Guid? departmentId = null,
        CancellationToken cancellationToken = default);

    Task<DoctorWithNavigationProperties> GetWithNavigationPropertiesAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<List<DoctorWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
        string? filterText = null,
        string? firstName = null,
        string? lastName = null,
        string? identityNumber = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        EnumGender? gender = null,
        string? email = null,
        string? phoneNumber = null,
        int? yearOfExperience = 0,
        string? city = null,
        string? district = null,
        Guid? titleId = null,
        Guid? departmentId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    );
    Task<List<Doctor>> GetListAsync(
        string? filterText = null,
        string? firstName = null,
        string? lastName = null,
        string? identityNumber = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        EnumGender? gender = null,
        string? email = null,
        string? phoneNumber = null,
        int? yearOfExperience = 0,
        string? city = null,
        string? district = null,
        Guid? titleId = null,
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
        int? yearOfExperience = 0,
        string? city = null,
        string? district = null,
        Guid? titleId = null,
        Guid? departmentId = null,
        CancellationToken cancellationToken = default);

    Task<long> GetCountByDepartmentIdsAsync(
        string? filterText = null,
        List<Guid>? departmentIds = null,
        CancellationToken cancellationToken = default);

    Task<List<DoctorWithNavigationProperties>> GetListByDepartmentIdsAsync(
        string? filterText = null,
        List<Guid>? departmentIds = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);
}