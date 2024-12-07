using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Pusula.Training.HealthCare.GlobalExceptions;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public class DoctorLeaveManager(IDoctorLeaveRepository doctorLeaveRepository) : DomainService, IDoctorLeaveManager
{
     public virtual async Task<DoctorLeave> CreateAsync( Guid doctorId, DateTime startDate, DateTime endDate, string? reason=null )
    {
            Check.NotNull(doctorId, nameof(doctorId));
            Check.NotNull(startDate, nameof(startDate));
            Check.NotNull(endDate, nameof(endDate));

            HealthCareGlobalException.ThrowIf(HealthCareDomainErrorCodes.InvalidDateRange_MESSAGE, 
                HealthCareDomainErrorCodes.InvalidDateRange_CODE, 
                startDate > endDate);
            
            var leaves = new DoctorLeave(
                GuidGenerator.Create(), doctorId, startDate, endDate, reason);

            return await doctorLeaveRepository.InsertAsync(leaves);
    
   
    }

    public virtual async Task<DoctorLeave> UpdateAsync( Guid id, Guid doctorId,
        DateTime startDate, DateTime endDate, string? reason = null,  [CanBeNull] string? concurrencyStamp = null)
    {
       
             Check.NotNull(doctorId, nameof(doctorId));
             Check.NotNull(startDate, nameof(startDate));
             Check.NotNull(endDate, nameof(endDate));
             
             HealthCareGlobalException.ThrowIf(HealthCareDomainErrorCodes.InvalidDateRange_MESSAGE, 
                 HealthCareDomainErrorCodes.InvalidDateRange_CODE, 
                 startDate > endDate);
        
             var leaves = await doctorLeaveRepository.GetAsync(id);
        
             leaves.SetDoctorId(doctorId);
             leaves.SetStartDate(startDate);
             leaves.SetEndDate(endDate);
             leaves.SetReason(reason);
        
             leaves.SetConcurrencyStampIfNotNull(concurrencyStamp);
             return await doctorLeaveRepository.UpdateAsync(leaves);
    
       
    }
        

}