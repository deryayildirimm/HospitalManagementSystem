using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.BloodTests
{
    public class BloodTestManager(IBloodTestRepository bloodTestRepository) : DomainService, IBloodTestManager
    {
        public virtual async Task<BloodTest> CreateAsync(Guid doctorId, Guid patientId, Guid testCategoryId, BloodTestStatus status, DateTime dateCreated, DateTime? dateCompleted = null)
        {
            Check.NotNullOrWhiteSpace(doctorId.ToString(), nameof(doctorId));
            Check.NotNullOrWhiteSpace(patientId.ToString(), nameof(patientId));
            Check.NotNullOrWhiteSpace(testCategoryId.ToString(), nameof(testCategoryId));
            Check.NotNullOrWhiteSpace(dateCreated.ToString(), nameof(dateCreated));
            Check.Range((int)status, nameof(status), BloodTestConst.BloodTestStatusMin, BloodTestConst.BloodTestStatusMax);

            var bloodTest = new BloodTest(GuidGenerator.Create(), doctorId, patientId, testCategoryId, status, dateCreated);

            return await bloodTestRepository.InsertAsync(bloodTest);
        }

        public virtual async Task<BloodTest> UpdateAsync(Guid id, Guid doctorId, Guid patientId, BloodTestStatus status, DateTime dateCreated, DateTime? dateCompleted = null)
        {
            Check.NotNullOrWhiteSpace(doctorId.ToString(), nameof(doctorId));
            Check.NotNullOrWhiteSpace(patientId.ToString(), nameof(patientId));
            Check.NotNullOrWhiteSpace(dateCreated.ToString(), nameof(dateCreated));
            Check.Range((int)status, nameof(status), BloodTestConst.BloodTestStatusMin, BloodTestConst.BloodTestStatusMax);

            var bloodTest = await bloodTestRepository.GetAsync(id);

            bloodTest.DoctorId = doctorId;
            bloodTest.PatientId = patientId;
            bloodTest.Status = status;
            bloodTest.DateCreated = dateCreated;
            bloodTest.DateCompleted = (DateTime)dateCompleted!;

            return await bloodTestRepository.UpdateAsync(bloodTest);
        }
    }
}
