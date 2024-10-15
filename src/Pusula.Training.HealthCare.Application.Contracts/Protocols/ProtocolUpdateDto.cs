using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.Protocols
{
    public class ProtocolUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        [StringLength(ProtocolConsts.TypeMaxLength, MinimumLength = ProtocolConsts.TypeMinLength)]
        public string Type { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public string? EndTime { get; set; }
        public Guid PatientId { get; set; }
        public Guid DepartmentId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}