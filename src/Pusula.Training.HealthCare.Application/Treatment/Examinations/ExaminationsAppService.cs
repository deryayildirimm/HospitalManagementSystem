using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Treatment.Icds;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.EventBus.Distributed;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.Examinations.Default)]
public class ExaminationsAppService(IExaminationRepository examinationRepository,
    ExaminationManager examinationManager,
    IDistributedEventBus distributedEventBus) : HealthCareAppService, IExaminationsAppService
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
        return ObjectMapper.Map<Examination, ExaminationDto>(examination!);
    }
    
    public async Task<PagedResultDto<IcdReportDto>> GetIcdReportAsync(GetIcdReportInput input)
    {
        var items = await examinationRepository.GetIcdReportAsync(input.StartDate, input.EndDate, input.FilterText, 
            input.CodeNumber, input.Detail, input.Sorting, input.MaxResultCount, input.SkipCount);
        var totalCount = await examinationRepository.GetIcdReportCountAsync(input.StartDate, input.EndDate, input.FilterText, 
            input.CodeNumber, input.Detail);
        return new PagedResultDto<IcdReportDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<IcdReport>, List<IcdReportDto>>(items)
        };
    }

    [Authorize(HealthCarePermissions.Examinations.Create)]
    public virtual async Task<ExaminationDto> CreateAsync(ExaminationCreateDto input)
    {
        var examination = await examinationManager.CreateAsync(input.ProtocolId, input.Date,
            input.Complaint, input.FamilyHistory.AreParentsRelated, input.FamilyHistory.MotherDisease,
            input.FamilyHistory.FatherDisease, input.FamilyHistory.SisterDisease, input.FamilyHistory.BrotherDisease,
            input.Background.Allergies, input.Background.Medications, input.Background.Habits, input.PhysicalFinding.Weight,
            input.PhysicalFinding.Height, input.PhysicalFinding.BodyTemperature, input.PhysicalFinding.Pulse,
            input.PhysicalFinding.Vki, input.PhysicalFinding.Vya, input.PhysicalFinding.Kbs, input.PhysicalFinding.Kbd,
            input.PhysicalFinding.Spo2, input.StartDate, input.Story, input.IcdIds);
        
        await distributedEventBus.PublishAsync(new ExaminationCreatedEto 
            { 
                Id = examination.Id, 
                FamilyHistoryId = examination.FamilyHistory!.Id,
                BackgroundId = examination.Background!.Id,
                CreatedAt = Clock.Now 
            },
            onUnitOfWorkComplete: false);
        
        return ObjectMapper.Map<Examination, ExaminationDto>(examination);
    }

    [Authorize(HealthCarePermissions.Examinations.Edit)]
    public virtual async Task<ExaminationDto> UpdateAsync(ExaminationUpdateDto input)
    {
        var examination = await examinationManager.UpdateAsync(input.Id, input.ProtocolId,
            input.Date, input.Complaint, input.FamilyHistory.AreParentsRelated, 
            input.FamilyHistory.MotherDisease, input.FamilyHistory.FatherDisease, input.FamilyHistory.SisterDisease, 
            input.FamilyHistory.BrotherDisease, input.Background.Allergies, input.Background.Medications, 
            input.Background.Habits, input.PhysicalFinding.Weight, input.PhysicalFinding.Height, 
            input.PhysicalFinding.BodyTemperature, input.PhysicalFinding.Pulse, input.PhysicalFinding.Vki, 
            input.PhysicalFinding.Vya, input.PhysicalFinding.Kbs, input.PhysicalFinding.Kbd,
            input.PhysicalFinding.Spo2, input.StartDate, input.Story, input.IcdIds);
        
        return ObjectMapper.Map<Examination, ExaminationDto>(examination);
    }
}