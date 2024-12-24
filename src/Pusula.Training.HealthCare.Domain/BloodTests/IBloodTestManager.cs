using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.BloodTests
{
    public interface IBloodTestManager : IDomainService
    {
        Task<BloodTest> CreateAsync(Guid doctorId, Guid patientId, BloodTestStatus status, DateTime dateCreated, DateTime? dateCompleted = null, List<Guid>? testCategoryIds = null);
        Task<BloodTest> UpdateAsync(Guid id, Guid doctorId, Guid patientId, BloodTestStatus status, DateTime dateCreated, DateTime? dateCompleted = null, List<Guid>? testCategoryIds = null);
    }
}
