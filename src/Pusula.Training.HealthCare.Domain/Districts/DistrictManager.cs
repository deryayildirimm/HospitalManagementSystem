using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Districts;

public class DistrictManager(IDistrictRepository cityRepository) : DomainService
{
    public virtual async Task<District> CreateAsync(Guid cityId, string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), DistrictConsts.NameMaxLength, DistrictConsts.NameMinLength);
        
        var city = new District(GuidGenerator.Create(), cityId, name);
        
        return await cityRepository.InsertAsync(city);
    }

    public virtual async Task<District> UpdateAsync(Guid id, Guid cityId, string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), DistrictConsts.NameMaxLength, DistrictConsts.NameMinLength);
        Check.NotNullOrWhiteSpace(cityId.ToString(), nameof(cityId));
        
        var city = await cityRepository.GetAsync(id);
        
        city.SetName(name);
        city.SetCityId(cityId);
        
        return await cityRepository.UpdateAsync(city);
    }
}