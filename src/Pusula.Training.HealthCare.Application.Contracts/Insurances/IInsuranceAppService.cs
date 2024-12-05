using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Pusula.Training.HealthCare.Insurances
{
    public interface IInsuranceAppService : IApplicationService
    {
        Task<PagedResultDto<InsuranceDto>> GetListAsync(GetInsurancesInput input);

        Task<InsuranceDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<InsuranceDto> CreateAsync(InsuranceCreateDto input);

        Task<InsuranceDto> UpdateAsync(Guid id, InsuranceUpdateDto input);

        Task DeleteByIdsAsync(List<Guid> insuranceIds);

        Task DeleteAllAsync(GetInsurancesInput input);
    }
}
