using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Insurances;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Controllers.Insurances
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Insurance")]
    [Route("api/app/insurances")]
    public class InsuranceController(IInsuranceAppService insuranceAppService) : HealthCareController, IInsuranceAppService
    {
        [HttpGet]
        [Route("{id}")]
        public Task<InsuranceDto> GetAsync(Guid id) => insuranceAppService.GetAsync(id);

        [HttpGet]
        public Task<PagedResultDto<InsuranceDto>> GetListAsync(GetInsurancesInput input) => insuranceAppService.GetListAsync(input);
        
        [HttpPost]
        public Task<InsuranceDto> CreateAsync(InsuranceCreateDto input) => insuranceAppService.CreateAsync(input);

        [HttpPut]
        [Route("{id}")]
        public Task<InsuranceDto> UpdateAsync(Guid id, InsuranceUpdateDto input) 
        {
            var a = 0;
            return insuranceAppService.UpdateAsync(id, input);
            
        }

        [HttpDelete]
        [Route("all")]
        public Task DeleteAllAsync(GetInsurancesInput input) => insuranceAppService.DeleteAllAsync(input);

        [HttpDelete]
        [Route("{id}")]
        public Task DeleteAsync(Guid id) => insuranceAppService.DeleteAsync(id);
        
        [HttpDelete]
        [Route("")]
        public Task DeleteByIdsAsync(List<Guid> insuranceIds) => insuranceAppService.DeleteByIdsAsync(insuranceIds);
    }
}
