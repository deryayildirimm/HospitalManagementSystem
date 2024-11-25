using AutoMapper;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Shared;
using System;
using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.Districts;
using Pusula.Training.HealthCare.DoctorLeaves;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.MedicalPersonnel;
using Pusula.Training.HealthCare.Titles;

namespace Pusula.Training.HealthCare;

public class HealthCareApplicationAutoMapperProfile : Profile
{
    public HealthCareApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<Patient, PatientDto>();
        CreateMap<Patient, PatientExcelDto>();
        CreateMap<PatientDto, PatientUpdateDto>();
        CreateMap<Patient, LookupDto<Guid>>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FirstName));

        CreateMap<Protocol, ProtocolDto>();
        CreateMap<Protocol, ProtocolExcelDto>();
        CreateMap<ProtocolDto, ProtocolUpdateDto>();
        CreateMap<ProtocolWithNavigationProperties, ProtocolWithNavigationPropertiesDto>();

        CreateMap<Department, DepartmentDto>();
        CreateMap<Department, DepartmentExcelDto>();
        CreateMap<DepartmentDto, DepartmentUpdateDto>();
        CreateMap<Department, LookupDto<Guid>>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<MedicalService, MedicalServiceDto>();
        CreateMap<MedicalService, MedicalServiceExcelDto>();
        CreateMap<MedicalServiceDto, MedicalServiceUpdateDto>();
        CreateMap<Department, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
        CreateMap<MedicalServiceWithDepartments, MedicalServiceWithDepartmentsDto>();
        
        CreateMap<Title, TitleDto>();
        CreateMap<TitleDto, TitleUpdateDto>();
        CreateMap<Title, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.TitleName));

        CreateMap<Doctor, DoctorDto>();
        CreateMap<Doctor, DoctorExcelDto>();
        CreateMap<DoctorDto, DoctorUpdateDto>();
        CreateMap<DoctorWithNavigationPropertiesDto, DoctorUpdateDto>();
        CreateMap<DoctorWithNavigationProperties, DoctorWithNavigationPropertiesDto>();
        
        CreateMap<DoctorLeave, DoctorLeaveDto>();
        CreateMap<DoctorLeave, DoctorLeaveExcelDto>();
        CreateMap<DoctorLeaveDto, DoctorLeaveUpdateDto>();
        
        CreateMap<MedicalStaff, MedicalStaffDto>();
        CreateMap<MedicalStaff, MedicalStaffExcelDto>();
        CreateMap<MedicalStaffDto, MedicalStaffUpdateDto>();
        CreateMap<MedicalStaffWithNavigationProperties, MedicalStaffWithNavigationPropertiesDto>();
        
        CreateMap<City, CityDto>();
        CreateMap<City, CityExcelDto>();
        CreateMap<CityDto, CityUpdateDto>();
        CreateMap<City, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<District, DistrictDto>();
        CreateMap<District, DistrictExcelDto>();
        CreateMap<DistrictDto, DistrictUpdateDto>();
        CreateMap<DistrictWithNavigationProperties, DistrictWithNavigationPropertiesDto>();
        CreateMap<District, LookupDto<Guid>>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
        
        CreateMap<Appointment, AppointmentDto>();
        CreateMap<AppointmentWithNavigationProperties, AppointmentWithNavigationPropertiesDto>();
        
        
        CreateMap<AppointmentWithNavigationProperties, AppointmentExcelDto>()
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.FirstName + " " + src.Doctor.LastName))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.FirstName + " " + src.Patient.LastName))
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.MedicalService.Name))
            .ForMember(dest => dest.PatientNumber, opt => opt.MapFrom(src => src.Patient.PatientNumber))
            .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src =>
                $"{src.Appointment.StartTime:yyyy-MM-dd}, {src.Appointment.StartTime:HH:mm} - {src.Appointment.EndTime:HH:mm}"));
        
    }
}
