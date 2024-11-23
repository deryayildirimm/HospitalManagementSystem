using System;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.MedicalPersonnel;

public interface IMedicalStaffManager : IDomainService
{
    Task<MedicalStaff> CreateAsync(
        Guid cityId,
        Guid districtId,
        Guid departmentId,
        string firstName,
        string lastName,
        string identityNumber,
        DateTime birthDate,
        EnumGender gender,
        DateTime startDate,
        string? email = null,
        string? phoneNumber = null);

    Task<MedicalStaff> UpdateAsync(
        Guid id,
        Guid cityId,
        Guid districtId,
        Guid departmentId,
        string firstName,
        string lastName,
        string identityNumber,
        DateTime birthDate,
        EnumGender gender,
        DateTime startDate,
        string? email = null,
        string? phoneNumber = null);
}