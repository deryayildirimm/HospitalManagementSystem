using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public interface ITestManager : IDomainService
    {
        Task<Test> CreateAsync(Guid testCategoryId, string name, double minValue, double maxValue);

    }
}
