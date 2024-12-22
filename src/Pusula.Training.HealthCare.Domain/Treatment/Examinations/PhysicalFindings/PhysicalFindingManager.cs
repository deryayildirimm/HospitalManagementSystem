using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Treatment.Examinations.PhysicalFindings;

public class PhysicalFindingManager(IPhysicalFindingRepository physicalFindingRepository) : DomainService, IPhysicalFindingManager
{
    public virtual async Task<PhysicalFinding> CreateAsync(
        Guid examinationId,
        int? weight,
        int? height,
        int? bodyTemperature,
        int? pulse,
        int? vki,
        int? vya,
        int? kbs,
        int? kbd,
        int? spo2)
    {
        Check.NotNullOrWhiteSpace(examinationId.ToString(), nameof(examinationId));
        
        var physicalFinding = new PhysicalFinding(
            Guid.NewGuid(),
            examinationId, 
            weight,
            height,
            bodyTemperature,
            pulse,
            vki,
            vya,
            kbs,
            kbd,
            spo2
        );
        return await physicalFindingRepository.InsertAsync(physicalFinding);
    }

    public virtual async Task<PhysicalFinding> UpdateAsync(
        Guid id,
        Guid examinationId,
        int? weight,
        int? height,
        int? bodyTemperature,
        int? pulse,
        int? vki,
        int? vya,
        int? kbs,
        int? kbd,
        int? spo2
    )
    {
        Check.NotNullOrWhiteSpace(examinationId.ToString(), nameof(examinationId));
        
        var physicalFinding = await physicalFindingRepository.GetAsync(id);
        
        physicalFinding.SetWeight(weight);
        physicalFinding.SetHeight(height);
        physicalFinding.SetBodyTemperature(bodyTemperature);
        physicalFinding.SetPulse(pulse);
        physicalFinding.SetVki(vki);
        physicalFinding.SetVya(vya);
        physicalFinding.SetKbs(kbs);
        physicalFinding.SetKbd(kbd);
        physicalFinding.SetSpo2(spo2);
        physicalFinding.SetExaminationId(examinationId);
        
        return await physicalFindingRepository.UpdateAsync(physicalFinding);
    }
}