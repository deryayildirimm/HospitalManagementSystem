using System;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Treatment.Icds;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public interface IExaminationsAppService
{
    Task<PagedResultDto<ExaminationDto>> GetListAsync(GetExaminationsInput input);
    Task<ExaminationDto> GetAsync(Guid id);
    Task<ExaminationDto?> GetByProtocolIdAsync(Guid? protocolId);
    Task<PagedResultDto<IcdReportDto>> GetIcdReportAsync(GetIcdReportInput input);

    Task<ExaminationDto> CreateAsync(ExaminationCreateDto input);
    Task<ExaminationDto> UpdateAsync(ExaminationUpdateDto input);
}