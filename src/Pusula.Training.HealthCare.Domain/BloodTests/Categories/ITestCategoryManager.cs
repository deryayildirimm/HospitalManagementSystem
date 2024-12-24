using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.BloodTests.Categories
{
    public interface ITestCategoryManager : IDomainService
    {
        Task<TestCategory> CreateAsync(string name, string description, string url, double price);
        Task<TestCategory> UpdateAsync(Guid id, string name, string description, string url, double price);
    }
}
