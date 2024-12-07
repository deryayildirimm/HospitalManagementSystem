using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Permissions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.EventBus.Distributed;
using static Pusula.Training.HealthCare.Permissions.HealthCarePermissions;

namespace Pusula.Training.HealthCare.Insurances
{
    [RemoteService(IsEnabled = false)]
    [Authorize(HealthCarePermissions.Insurances.Default)]
    public class InsuranceAppService(
        IInsuranceRepository insuranceRepository, IInsuranceManager insuranceManager,
        IDistributedEventBus distributedEventBus) : HealthCareAppService, IInsuranceAppService
    {
        public virtual async Task<PagedResultDto<InsuranceDto>> GetListAsync(GetInsurancesInput input)
        {
            var items = await insuranceRepository.GetListAsync(input.FilterText, input.PolicyNumber, input.PremiumAmount, input.CoverageAmount, input.StartDateMin, input.StartDateMax, 
                input.EndDateMin, input.EndDateMax, input.InsuranceCompanyName, input.Description, input.Sorting, input.MaxResultCount, input.SkipCount);

            var count = await insuranceRepository.GetCountAsync(input.FilterText, input.PolicyNumber, input.PremiumAmount, input.CoverageAmount, input.StartDateMin, input.StartDateMax, 
                input.EndDateMin, input.EndDateMax, input.InsuranceCompanyName, input.Description);

            return new PagedResultDto<InsuranceDto>
            {
                TotalCount = count,
                Items = ObjectMapper.Map<List<Insurance>, List<InsuranceDto>>(items)
            };
        }

        public virtual async Task<InsuranceDto> GetAsync(Guid id) => ObjectMapper.Map<Insurance, InsuranceDto>(await insuranceRepository.GetAsync(id));

        [Authorize(HealthCarePermissions.Insurances.Create)]
        public virtual async Task<InsuranceDto> CreateAsync(InsuranceCreateDto input)
        {
            var insurance = await insuranceManager.CreateAsync(input.PolicyNumber, input.InsuranceCompanyName, input.PremiumAmount, input.CoverageAmount,
                input.StartDate, input.EndDate, input.Description);

            await distributedEventBus.PublishAsync(new InsuranceCreatedEto { InsuranceCompanyName = input.InsuranceCompanyName.ToString(), PolicyNumber = input.PolicyNumber, 
                CreatedDate = Clock.Now }, onUnitOfWorkComplete: false);

            return ObjectMapper.Map<Insurance, InsuranceDto>(insurance);
        }

        [Authorize(HealthCarePermissions.Insurances.Edit)]
        public virtual async Task<InsuranceDto> UpdateAsync(Guid id, InsuranceUpdateDto input)
        {
            var insurance = await insuranceManager.UpdateAsync(id, input.PolicyNumber, input.InsuranceCompanyName, input.PremiumAmount, input.CoverageAmount,
                input.StartDate, input.EndDate, input.Description);

            return ObjectMapper.Map<Insurance, InsuranceDto>(insurance);
        }

        [Authorize(HealthCarePermissions.Insurances.Delete)]
        public virtual async Task DeleteAllAsync(GetInsurancesInput input) => await insuranceRepository.DeleteAllAsync(input.FilterText, input.PolicyNumber, 
            input.PremiumAmount, input.CoverageAmount, input.StartDateMin, input.StartDateMax, input.EndDateMin, input.EndDateMax, input.InsuranceCompanyName);

        [Authorize(HealthCarePermissions.Insurances.Delete)]
        public virtual async Task DeleteAsync(Guid id) => await insuranceRepository.DeleteAsync(id);
        

        [Authorize(HealthCarePermissions.Insurances.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> insuranceIds) => await insuranceRepository.DeleteManyAsync(insuranceIds);
        
    }
}
