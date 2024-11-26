using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.BloodTests
{
    public interface IBloodTestRepository : IRepository<BloodTest, Guid>
    {
        Task<BloodTestWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<List<BloodTestWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            BloodTestStatus? status = null,
            DateTime? dateCreatedMin = null,
            DateTime? dateCreatedMax = null,
            DateTime? dateCompletedMin = null,
            DateTime? dateCompletedMax = null,
            Guid? doctorId = null,
            Guid? patientId = null,
            Guid? testCategoryId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
            );

        Task<List<BloodTest>> GetListAsync(
            string? filterText = null,
            BloodTestStatus? status = null,
            DateTime? dateCreatedMin = null,
            DateTime? dateCreatedMax = null,
            DateTime? dateCompletedMin = null,
            DateTime? dateCompletedMax = null,
            Guid? doctorId = null,
            Guid? patientId = null,
            Guid? testCategoryId = null,
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
            Guid? testCategoryId = null,
            CancellationToken cancellationToken = default
            );
    }
}
