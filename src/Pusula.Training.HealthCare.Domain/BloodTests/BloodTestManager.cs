using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.BloodTests
{
    public class BloodTestManager(IBloodTestRepository bloodTestRepository) : DomainService, IBloodTestManager
    {
        public virtual async Task<BloodTest> CreateAsync(Guid doctorId, Guid patientId, BloodTestStatus status, DateTime dateCreated, 
            DateTime? dateCompleted = null, List<Guid>? testCategoryIds = null)
        {
            Check.NotNullOrWhiteSpace(doctorId.ToString(), nameof(doctorId));
            Check.NotNullOrWhiteSpace(patientId.ToString(), nameof(patientId));
            Check.NotNullOrWhiteSpace(dateCreated.ToString(), nameof(dateCreated));
            Check.Range((int)status, nameof(status), BloodTestConst.BloodTestStatusMin, BloodTestConst.BloodTestStatusMax);

            var bloodTest = new BloodTest(GuidGenerator.Create(), doctorId, patientId, status, dateCreated);

            await SetCategoriesAsync(bloodTest, testCategoryIds!);

            await bloodTestRepository.InsertAsync(bloodTest,true);
            
            return await bloodTestRepository.GetAsync(bloodTest.Id);
        }

        public virtual async Task<BloodTest> UpdateAsync(Guid id, Guid doctorId, Guid patientId, BloodTestStatus status, DateTime dateCreated, 
            DateTime? dateCompleted = null, List<Guid>? testCategoryIds = null)
        {
            Check.NotNullOrWhiteSpace(doctorId.ToString(), nameof(doctorId));
            Check.NotNullOrWhiteSpace(patientId.ToString(), nameof(patientId));
            Check.NotNullOrWhiteSpace(dateCreated.ToString(), nameof(dateCreated));
            Check.Range((int)status, nameof(status), BloodTestConst.BloodTestStatusMin, BloodTestConst.BloodTestStatusMax);

            var bloodTest = await bloodTestRepository.GetWithNavigationPropertiesAsync(id);

            bloodTest!.SetDoctorId(doctorId);
            bloodTest.SetPatientId(patientId);
            bloodTest.SetStatus(status);
            bloodTest.SetDateCreated(dateCreated);
            bloodTest.SetDateCompleted(dateCompleted);

            await SetCategoriesAsync(bloodTest, testCategoryIds!);

            return await bloodTestRepository.UpdateAsync(bloodTest);
        }
        private Task SetCategoriesAsync(BloodTest bloodTest, [CanBeNull] List<Guid> categoryIds)
        {
            if (categoryIds?.Any() != true)
            {
                bloodTest.RemoveAllCategories();
                return Task.CompletedTask; ;
            }

            bloodTest.RemoveAllCategoriesExceptGivenIds(categoryIds);

            foreach (var categoryId in categoryIds)
            {
                bloodTest.AddCategory(categoryId);
            }

            return Task.CompletedTask;
        }
    }
}
