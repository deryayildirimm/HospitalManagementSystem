using System;

namespace Pusula.Training.HealthCare.Blazor.Services;

public class ProtocolStateService
{
    public Guid ProtocolId { get; set; }

    public string ProtocolTypeName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PatientIdentityNumber { get; set; } = string.Empty;
    public string PatientGender { get; set; } = string.Empty;
    public DateTime PatientBirthDate { get; set; }

    public string DoctorName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;

    public bool HasValidProtocol { get; set; }

}