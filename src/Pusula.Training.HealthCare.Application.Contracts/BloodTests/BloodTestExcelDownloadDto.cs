using System;

namespace Pusula.Training.HealthCare.BloodTests
{
    public class BloodTestExcelDownloadDto
    {
        public string DownloadToken { get; set; } = null!;
        public string? FilterText { get; set; } 
        public BloodTestStatus? Status { get; set; }
        public DateTime? DateCreatedMin { get; set; }
        public DateTime? DateCreatedMax { get; set; }
        public DateTime? DateCompletedMin { get; set; }
        public DateTime? DateCompletedMax { get; set; }
        public Guid? DoctorId { get; set; }
        public Guid? PatientId { get; set; }
        public Guid? TestCategoryId { get; set; }
        public BloodTestExcelDownloadDto() { }
    }
}
