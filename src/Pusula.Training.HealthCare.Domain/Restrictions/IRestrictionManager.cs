using System;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Patients;

namespace Pusula.Training.HealthCare.Restrictions;

public interface IRestrictionManager
{
    Task<Restriction> CreateAsync(
        Guid medicalServiceId, 
        Guid departmentId,
        Guid? doctorId,
        int? minAge,
        int? maxAge,
        EnumGender allowedGender
    );
    
    Task<Restriction> UpdateAsync(
        Guid id,
        Guid medicalServiceId, 
        Guid departmentId,
        Guid? doctorId,
        int? minAge,
        int? maxAge,
        EnumGender allowedGender,
        string? concurrencyStamp = null
    );
}