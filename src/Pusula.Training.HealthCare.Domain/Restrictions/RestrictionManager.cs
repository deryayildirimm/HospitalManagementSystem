using System;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.GlobalExceptions;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Restrictions;

public class RestrictionManager(
    IRestrictionRepository restrictionRepository) : DomainService, IRestrictionManager
{
    public virtual async Task<Restriction> CreateAsync(Guid medicalServiceId, Guid departmentId, Guid doctorId, int? minAge, int? maxAge,
        EnumGender? allowedGender)
    {
        Check.NotNull(medicalServiceId, nameof(medicalServiceId));
        Check.NotNull(departmentId, nameof(departmentId));
        
          var restriction  = new Restriction(
            GuidGenerator.Create(),
            medicalServiceId,
            departmentId,
            doctorId,
            minAge,
            maxAge,
            allowedGender
        );
          
        return await restrictionRepository.InsertAsync(restriction);
        
    }

    public virtual async Task<Restriction> UpdateAsync(Guid id, Guid medicalServiceId, Guid departmentId, Guid doctorId, int? minAge, int? maxAge,
        EnumGender? allowedGender, string? concurrencyStamp = null)
    {
        Check.NotNull(medicalServiceId, nameof(medicalServiceId));
        Check.NotNull(departmentId, nameof(departmentId));
        
        var restriction = await restrictionRepository.FirstOrDefaultAsync(x => x.Id == id);
        HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.RestrictionNotFound, restriction is null);
        
        restriction!.SetMedicalServiceId(medicalServiceId);
        restriction!.SetDepartmentId(departmentId);
        restriction!.SetDoctorId(doctorId);
        restriction!.SetMinAge(minAge);
        restriction!.SetMaxAge(maxAge);
        restriction!.SetAllowedGender(allowedGender);
        
        return await restrictionRepository.UpdateAsync(restriction);
    }
}