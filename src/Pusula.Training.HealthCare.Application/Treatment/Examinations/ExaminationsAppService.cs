using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;
using Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.Examinations.Default)]
public class ExaminationsAppService(IExaminationRepository examinationRepository,
    ExaminationManager examinationManager) : HealthCareAppService, IExaminationsAppService
{
    public virtual async Task<PagedResultDto<ExaminationDto>> GetListAsync(GetExaminationsInput input)
    {
        var totalCount = await examinationRepository.GetCountAsync(input.FilterText, input.DateMin, 
            input.DateMax, input.Complaint, input.Story, input.ProtocolId);
        var items = await examinationRepository.GetListAsync(input.FilterText, input.DateMin, 
            input.DateMax, input.Complaint, input.Story, input.ProtocolId, 
            input.Sorting, input.MaxResultCount, input.SkipCount);
        return new PagedResultDto<ExaminationDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Examination>, List<ExaminationDto>>(items)
        };
    }

    public virtual async Task<ExaminationDto> GetAsync(Guid id)
    {
        var examination = await examinationRepository.GetAsync(id);
        
        return ObjectMapper.Map<Examination, ExaminationDto>(examination);
    }

    public virtual async Task<ExaminationDto?> GetByProtocolIdAsync(Guid? protocolId)
    {
        var examination = await examinationRepository.GetByProtocolIdAsync(protocolId);
        if (examination == null) return null;
        return ObjectMapper.Map<Examination, ExaminationDto>(examination);
    }

    [Authorize(HealthCarePermissions.Examinations.Delete)]
    public virtual async void DeleteAsync(Guid id)
    {
        await examinationRepository.DeleteAsync(id);
    }

    [Authorize(HealthCarePermissions.Examinations.Create)]
    public virtual async Task<ExaminationDto> CreateAsync(ExaminationCreateDto input)
    {
        var examination = await examinationManager.CreateAsync(input.ProtocolId,input.Date,
            input.Complaint, input.FamilyHistory.AreParentsRelated, input.FamilyHistory.MotherDisease,
            input.FamilyHistory.FatherDisease, input.FamilyHistory.SisterDisease, input.FamilyHistory.BrotherDisease,
            input.Background.Allergies, input.Background.Medications, input.Background.Habits, input.StartDate, 
            input.Story, input.IcdIds);
        
        return ObjectMapper.Map<Examination, ExaminationDto>(examination);
    }

    [Authorize(HealthCarePermissions.Examinations.Edit)]
    public virtual async Task<ExaminationDto> UpdateAsync(ExaminationUpdateDto input)
    {
        var examination = await examinationManager.UpdateAsync(input.Id, input.ProtocolId,
            input.Date, input.Complaint, input.FamilyHistory.AreParentsRelated, 
            input.FamilyHistory.MotherDisease, input.FamilyHistory.FatherDisease, input.FamilyHistory.SisterDisease, 
            input.FamilyHistory.BrotherDisease, input.Background.Allergies, input.Background.Medications, 
            input.Background.Habits, input.StartDate, input.Story, input.IcdIds);
        
        return ObjectMapper.Map<Examination, ExaminationDto>(examination);
    }

    [Authorize(HealthCarePermissions.Examinations.Delete)]
    public virtual async Task DeleteByIdsAsync(List<Guid> examinationIds)
    {
        await examinationRepository.DeleteManyAsync(examinationIds);
    }

    [Authorize(HealthCarePermissions.Examinations.Delete)]
    public virtual async Task DeleteAllAsync(GetExaminationsInput input)
    {
        await examinationRepository.DeleteAllAsync(input.FilterText, input.DateMin, input.DateMax, input.Complaint,
            input.Story, input.ProtocolId);
    }
}