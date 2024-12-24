using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.BloodTests
{
    public interface IBloodTestRepository : IRepository<BloodTest, Guid>
    {
        Task<BloodTest?> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<List<BloodTest>> GetListAsync(
            string? filterText = null,
            BloodTestStatus? status = null,
            DateTime? dateCreatedMin = null,
            DateTime? dateCreatedMax = null,
            DateTime? dateCompletedMin = null,
            DateTime? dateCompletedMax = null,
            Guid? doctorId = null,
            Guid? patientId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
            );

        Task<long> GetCountAsync(
            string? filterText = null,
            BloodTestStatus? status = null,
            DateTime? dateCreatedMin = null,
            DateTime? dateCreatedMax = null,
            DateTime? dateCompletedMin = null,
            DateTime? dateCompletedMax = null,
            Guid? doctorId = null,
            Guid? patientId = null,
            CancellationToken cancellationToken = default
            );

        Task<List<Guid>> GetCategoryIdsAsync(
            Guid id,
            CancellationToken cancellationToken = default
            );
    }
}
