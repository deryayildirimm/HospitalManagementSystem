using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.BloodTests
{
    public class GetBloodTestsInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }
        public BloodTestStatus? Status { get; set; }
        public DateTime? DateCreatedMin { get; set; }
        public DateTime? DateCreatedMax { get; set; }
        public DateTime? DateCompletedMin { get; set; }
        public DateTime? DateCompletedMax { get; set; }
        public Guid? DoctorId { get; set; }
        public Guid? PatientId { get; set; }
        public GetBloodTestsInput() { }
    }
}
