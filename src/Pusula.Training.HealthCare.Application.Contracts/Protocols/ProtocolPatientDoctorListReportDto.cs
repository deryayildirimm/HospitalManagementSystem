using System;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolPatientDoctorListReportDto
{
    public Guid PatientId { get; set; }
    public int  PatientNumber { get; set; }
    public string FullName { get; set; }
    public string DoctorName { get; set; }
    public int ProtocolCount { get; set; } // Açılan protokol sayısı
    public DateTime? LastVisit { get; set; } // Son ziyaret zamanı
}