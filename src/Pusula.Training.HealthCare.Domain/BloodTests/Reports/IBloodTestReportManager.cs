using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.BloodTests.Reports
{
    public interface IBloodTestReportManager : IDomainService
    {
        Task<BloodTestReport> CreateAsync(Guid bloodTestId, List<Guid>? bloodTestResultIds = null);
        Task<BloodTestReport> UpdateAsync(Guid id, Guid bloodTestId, List<Guid>? bloodTestResultIds = null);
    }
}
