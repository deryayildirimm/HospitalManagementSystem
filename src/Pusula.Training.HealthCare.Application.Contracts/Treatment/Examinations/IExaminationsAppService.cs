using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public interface IExaminationsAppService
{
    Task<PagedResultDto<ExaminationDto>> GetListAsync(GetExaminationsInput input);
    Task<ExaminationDto> GetAsync(Guid id);
    void DeleteAsync(Guid id);
    Task<ExaminationDto> CreateAsync(ExaminationCreateDto input);
    Task<ExaminationDto> UpdateAsync(ExaminationUpdateDto input);
    Task DeleteByIdsAsync(List<Guid> examinationIds);
    Task DeleteAllAsync(GetExaminationsInput input);
}