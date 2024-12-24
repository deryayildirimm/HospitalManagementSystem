using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.AppointmentTypes;
using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Districts;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Insurances;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.ProtocolTypes;
using Pusula.Training.HealthCare.Shared;
using Pusula.Training.HealthCare.Titles;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Lookups;

public class LookupAppService(
    IAppointmentTypeRepository appointmentTypeRepository,
    IMedicalServiceRepository medicalServiceRepository,
    IDepartmentRepository departmentRepository,
    ITitleRepository titleRepository,
    ICityRepository cityRepository,
    IDistrictRepository districtRepository,
    IInsuranceRepository insuranceRepository,
    IProtocolTypeRepository protocolTypeRepository,
    IDoctorRepository doctorRepository
) : HealthCareAppService, ILookupAppService
{
    public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetAppointmentTypeLookupAsync(LookupRequestDto input)
    {
        var query = (await appointmentTypeRepository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Name.Contains(input.Filter!));

        var lookupData =
            await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<AppointmentType>();
        var totalCount = query.Count();
        return new PagedResultDto<LookupDto<Guid>>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<AppointmentType>, List<LookupDto<Guid>>>(lookupData)
        };
    }

    public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetMedicalServiceLookupAsync(LookupRequestDto input)
    {
        var query = (await medicalServiceRepository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Name.Contains(input.Filter!));

        var lookupData =
            await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<MedicalService>();
        var totalCount = query.Count();
        return new PagedResultDto<LookupDto<Guid>>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<MedicalService>, List<LookupDto<Guid>>>(lookupData)
        };
    }

    public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input)
    {
        var query = (await departmentRepository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Name.Contains(input.Filter!));

        var lookupData =
            await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Department>();
        var totalCount = query.Count();
        return new PagedResultDto<LookupDto<Guid>>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Department>, List<LookupDto<Guid>>>(lookupData)
        };
    }
    
    public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetTitleLookupAsync(LookupRequestDto input)
    {
        var query = (await titleRepository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.TitleName.Contains(input.Filter!));

        var lookupData =
            await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Title>();
        var totalCount = query.Count();
        return new PagedResultDto<LookupDto<Guid>>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Title>, List<LookupDto<Guid>>>(lookupData)
        };
    }

    public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetCityLookupAsync(LookupRequestDto input)
    {
        var query = (await cityRepository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Name.Contains(input.Filter!));

        var lookupData =
            await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<City>();
        var totalCount = query.Count();
        return new PagedResultDto<LookupDto<Guid>>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<City>, List<LookupDto<Guid>>>(lookupData)
        };
    }

    public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetDistrictLookupAsync(LookupRequestDto input)
    {
        var query = (await districtRepository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Name.Contains(input.Filter!));

        var lookupData =
            await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<District>();
        var totalCount = query.Count();
        return new PagedResultDto<LookupDto<Guid>>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<District>, List<LookupDto<Guid>>>(lookupData)
        };
    }
    
    public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetInsuranceLookupAsync(LookupRequestDto input)
    {
        var query = (await insuranceRepository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x =>  x.InsuranceCompanyName.ToString().Contains(input.Filter!));

        var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Insurance>();
        var totalCount = query.Count();
        return new PagedResultDto<LookupDto<Guid>>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Insurance>, List<LookupDto<Guid>>>(lookupData)
        };
    }
    
    public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetProtocolTypeLookupAsync(LookupRequestDto input)
    {
        var query = (await protocolTypeRepository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Name.Contains(input.Filter!));
        var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<ProtocolType>();
        var totalCount = query.Count();

        return new PagedResultDto<LookupDto<Guid>>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<ProtocolType>, List<LookupDto<Guid>>>(lookupData)
        };
    }
        
    public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetDoctorLookupAsync(LookupRequestDto input)
    {
        var query = (await doctorRepository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x =>  x.FirstName.Contains(input.Filter!));

        var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Doctor>();
        var totalCount = query.Count();
        return new PagedResultDto<LookupDto<Guid>>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Doctor>, List<LookupDto<Guid>>>(lookupData)
        };
    }
    
    
    
}