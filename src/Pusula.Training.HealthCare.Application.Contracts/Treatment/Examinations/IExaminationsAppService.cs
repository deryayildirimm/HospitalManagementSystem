using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;
using Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public interface IExaminationsAppService
{
    Task<PagedResultDto<ExaminationDto>> GetListAsync(GetExaminationsInput input);
    Task<ExaminationDto> GetAsync(Guid id);
    Task<ExaminationDto?> GetByProtocolIdAsync(Guid? protocolId);
    void DeleteAsync(Guid id);
    Task<ExaminationDto> CreateAsync(ExaminationCreateDto input);
    Task<ExaminationDto> UpdateAsync(ExaminationUpdateDto input);
    Task DeleteByIdsAsync(List<Guid> examinationIds);
    Task DeleteAllAsync(GetExaminationsInput input);
}