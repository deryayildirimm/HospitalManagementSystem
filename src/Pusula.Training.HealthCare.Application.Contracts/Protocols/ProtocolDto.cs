using System;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.Protocols
{
    public class ProtocolDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Type { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public string? EndTime { get; set; }
        public Guid PatientId { get; set; }
        public Guid DepartmentId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}