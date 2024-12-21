using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;

public class Background : FullAuditedAggregateRoot<Guid>
{
    public string? Allergies { get; private set; }
    public string? Medications { get; private set; }
    public string? Habits { get; private set; }
    public Guid ExaminationId { get; private set; }
    public Examination Examination { get; private set; } = null!;

    protected Background()
    {
        ExaminationId = Guid.Empty;
    }

    public Background(Guid id, Guid examinationId, string? allergies = null, string? medications = null,
        string? habits = null)
    {
        Id = id;
        SetExaminationId(examinationId);
        SetAllergies(allergies);
        SetMedications(medications);
        SetHabits(habits);
    }

    public void SetAllergies(string? allergies)
    {
        Check.Length(allergies, nameof(allergies), BackgroundConsts.AllergiesMaxLength);
        Allergies = allergies;
    }

    public void SetMedications(string? medications)
    {
        Check.Length(medications, nameof(medications), BackgroundConsts.MedicationsMaxLength);
        Medications = medications;
    }

    public void SetHabits(string? habits)
    {
        Check.Length(habits, nameof(habits), BackgroundConsts.HabitsMaxLength);
        Habits = habits;
    }
    
    public void SetExaminationId(Guid examinationId)
    {
        Check.NotNullOrWhiteSpace(examinationId.ToString(), nameof(examinationId));
        ExaminationId = examinationId;
    }
}