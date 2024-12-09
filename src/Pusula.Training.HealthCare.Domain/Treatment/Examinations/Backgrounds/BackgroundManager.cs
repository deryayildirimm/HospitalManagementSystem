using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;

public class BackgroundManager(IBackgroundRepository backgroundRepository) : DomainService
{
    public virtual async Task<Background> CreateAsync(
        Guid examinationId,
        string? allergies,
        string? medications,
        string? habits
        )
    {
        Check.NotNullOrWhiteSpace(examinationId.ToString(), nameof(examinationId));
        Check.Length(allergies, nameof(allergies), BackgroundConsts.AllergiesMaxLength);
        Check.Length(medications, nameof(medications), BackgroundConsts.MedicationsMaxLength);
        Check.Length(habits, nameof(habits), BackgroundConsts.HabitsMaxLength);

        var background = new Background(
            Guid.NewGuid(),
            examinationId, 
            allergies, 
            medications, 
            habits
        );
        return await backgroundRepository.InsertAsync(background);
    }

    public virtual async Task<Background> UpdateAsync(
        Guid id,
        Guid examinationId,
        string? allergies,
        string? medications,
        string? habits
    )
    {
        Check.NotNullOrWhiteSpace(examinationId.ToString(), nameof(examinationId));
        Check.Length(allergies, nameof(allergies), BackgroundConsts.AllergiesMaxLength);
        Check.Length(medications, nameof(medications), BackgroundConsts.MedicationsMaxLength);
        Check.Length(habits, nameof(habits), BackgroundConsts.HabitsMaxLength);
        
        var background = await backgroundRepository.GetAsync(id);
        
        background.SetExaminationId(examinationId);
        background.SetAllergies(allergies);
        background.SetMedications(medications);
        background.SetHabits(habits);
        
        return await backgroundRepository.UpdateAsync(background);
    }
}