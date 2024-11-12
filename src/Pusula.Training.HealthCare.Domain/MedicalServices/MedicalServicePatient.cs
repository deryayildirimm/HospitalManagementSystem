using System;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.MedicalServices;

public class MedicalServicePatient : Entity
{
    public Guid MedicalServiceId { get; set; }
    public Guid PatientId { get; set; }

    public double Amount { get; set; }

    public DateTime ActionTime { get; set; }

    private MedicalServicePatient()
    {
    }

    public MedicalServicePatient(Guid medicalServiceId, Guid patientId, double amount, DateTime actionTime)
    {
        Check.NotNull(medicalServiceId, nameof(medicalServiceId));
        Check.NotNull(patientId, nameof(patientId));
        Check.NotNull(actionTime, nameof(actionTime));
        Check.Range(amount, nameof(amount), MedicalServiceConsts.CostMinValue);

        MedicalServiceId = medicalServiceId;
        PatientId = patientId;
        Amount = amount;
        ActionTime = actionTime;
    }

    public override object?[] GetKeys()
    {
        return new object?[] { MedicalServiceId, PatientId };
    }
}