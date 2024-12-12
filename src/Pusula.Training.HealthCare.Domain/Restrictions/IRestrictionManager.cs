using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Restrictions;

namespace Pusula.Training.HealthCare.MedicalServices;

public interface IRestrictionManager
{
    Task<Restriction> CreateAsync(
        Guid medicalServiceId, 
        Guid departmentId,
        Guid doctorId,
        int? minAge,
        int? maxAge,
        EnumGender? allowedGender
    );
    
    Task<Restriction> UpdateAsync(
        Guid id,
        Guid medicalServiceId, 
        Guid departmentId,
        Guid doctorId,
        int? minAge,
        int? maxAge,
        EnumGender? allowedGender,
        string? concurrencyStamp = null
    );
}