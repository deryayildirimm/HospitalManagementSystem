using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Cities;

public class CityManager(ICityRepository cityRepository) : DomainService
{
    public virtual async Task<City> CreateAsync(string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), CityConsts.NameMaxLength, CityConsts.NameMinLength);
        
        var city = new City(GuidGenerator.Create(), name);
        
        return await cityRepository.InsertAsync(city);
    }

    public virtual async Task<City> UpdateAsync(Guid id, string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), CityConsts.NameMaxLength, CityConsts.NameMinLength);
        
        var city = await cityRepository.GetAsync(id);
        
        city.SetName(name);
        
        return await cityRepository.UpdateAsync(city);
    }
}