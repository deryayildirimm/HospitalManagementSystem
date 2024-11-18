using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public class DoctorLeaveManager(IDoctorLeaveRepository repo) : DomainService
{
     public virtual async Task<DoctorLeave> CreateAsync( Guid doctorId, DateTime startDate, DateTime endDate, string? reason=null )
    {
       
        Check.NotNullOrWhiteSpace(doctorId.ToString(), nameof(doctorId));
        Check.NotNull(startDate, nameof(startDate));
        Check.NotNull(endDate, nameof(endDate));

        var leaves = new DoctorLeave(
            GuidGenerator.Create(), doctorId, startDate, endDate, reason);

        return await repo.InsertAsync(leaves);

   
    }

    public virtual async Task<DoctorLeave> UpdateAsync( Guid id, Guid doctorId,
        DateTime startDate, DateTime endDate, string? reason = null,  [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNullOrWhiteSpace(doctorId.ToString(), nameof(doctorId));
        Check.NotNull(startDate, nameof(startDate));
        Check.NotNull(endDate, nameof(endDate));

        var leaves = await repo.GetAsync(id);
        
        leaves.DoctorId = doctorId;
        leaves.StartDate = startDate;
        leaves.EndDate = endDate;
        leaves.Reason = reason;
        
        leaves.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await repo.UpdateAsync(leaves);

       
    }
        

}