using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.BloodTests.Categories
{
    public interface ITestCategoryRepository : IRepository<TestCategory,Guid>
    {
        Task<List<TestCategory>> GetListAsync(string? filterText, string? name, string? description, string? url, double? price, string? sorting = null, int maxResultCount = int.MaxValue, 
            int skipCount = 0, CancellationToken cancellationToken = default);

        Task<long> GetCountAsync(string? filterText, string? name, string? description, string? url, double? price, CancellationToken cancellationToken = default);
    }
}
