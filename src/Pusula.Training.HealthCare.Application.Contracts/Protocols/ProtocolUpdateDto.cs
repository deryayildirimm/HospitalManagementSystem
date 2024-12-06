using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.Protocols
{
    public class ProtocolUpdateDto : IHasConcurrencyStamp
    {
       
       
        public string? Notes { get; set; } 
        [Required] public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Guid PatientId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid ProtocolTypeId { get; set; }
        public Guid DoctorId { get; set; }
        public string ConcurrencyStamp { get; set; } = null!;
    }
}