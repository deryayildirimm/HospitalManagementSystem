using AutoMapper;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Shared;
using System;
using Pusula.Training.HealthCare.AppointmentTypes;
using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.Districts;
using Pusula.Training.HealthCare.DoctorLeaves;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.MedicalPersonnel;
using Pusula.Training.HealthCare.Titles;
using Pusula.Training.HealthCare.BloodTests;
using Pusula.Training.HealthCare.BloodTests.Categories;
using Pusula.Training.HealthCare.BloodTests.Category;
using Pusula.Training.HealthCare.BloodTests.Tests;
using Pusula.Training.HealthCare.Treatment.Examinations;
using Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;
using Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;
using Pusula.Training.HealthCare.DoctorWorkingHours;
using Pusula.Training.HealthCare.Treatment.Icds;
using Pusula.Training.HealthCare.ProtocolTypes;
using Pusula.Training.HealthCare.Insurances;

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
        CreateMap<ProtocolPatientDepartmentListReport, ProtocolPatientDepartmentListReportDto>();
        CreateMap<ProtocolPatientDoctorListReport, ProtocolPatientDoctorListReportDto>();
        CreateMap<ProtocolDto, ProtocolUpdateDto>();
        CreateMap<DepartmentStatistic, DepartmentStatisticDto>();
        CreateMap<DoctorStatistics, DoctorStatisticDto>();
        CreateMap<ProtocolWithNavigationProperties, ProtocolWithNavigationPropertiesDto>();

        CreateMap<ProtocolType, ProtocolTypeDto>();
        CreateMap<ProtocolTypeDto, ProtocolTypeUpdateDto>();
        CreateMap<ProtocolType, LookupDto<Guid>>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<Department, DepartmentDto>();
        CreateMap<Department, DepartmentExcelDto>();
        CreateMap<DepartmentDto, DepartmentUpdateDto>();
        CreateMap<Department, LookupDto<Guid>>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<MedicalService, MedicalServiceDto>();
        CreateMap<MedicalService, MedicalServiceExcelDto>();
        CreateMap<MedicalServiceDto, MedicalServiceUpdateDto>();
        CreateMap<MedicalServiceWithDepartments, MedicalServiceWithDepartmentsDto>();
        CreateMap<MedicalService, LookupDto<Guid>>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<Title, TitleDto>();
        CreateMap<TitleDto, TitleUpdateDto>();
        CreateMap<Title, LookupDto<Guid>>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.TitleName));

        CreateMap<Doctor, DoctorDto>();
        CreateMap<Doctor, DoctorExcelDto>();
        CreateMap<DoctorDto, DoctorUpdateDto>();
        CreateMap<DoctorWithNavigationPropertiesDto, DoctorUpdateDto>();
        CreateMap<DoctorWithNavigationProperties, DoctorWithNavigationPropertiesDto>();
        CreateMap<Doctor, LookupDto<Guid>>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FirstName));

        CreateMap<DoctorLeave, DoctorLeaveDto>();
        CreateMap<DoctorLeave, DoctorLeaveExcelDto>();
        CreateMap<DoctorLeaveDto, DoctorLeaveUpdateDto>();
        CreateMap<DoctorWithNavigationPropertiesDto, DoctorLookupDto<Guid>>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Doctor.Id))
            .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.Doctor.DepartmentId))
            .ForMember(dest => dest.DisplayName,
                opt => opt.MapFrom(src => $"{src.Doctor.FirstName} {src.Doctor.LastName}"));

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

        CreateMap<Icd, IcdDto>();
        CreateMap<Icd, IcdExcelDto>();
        CreateMap<IcdDto, IcdExcelDto>();
        CreateMap<IcdDto, IcdUpdateDto>();
        CreateMap<Icd, LookupDto<Guid>>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.CodeNumber));
        CreateMap<Icd, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.CodeNumber));
        
        CreateMap<FamilyHistory, FamilyHistoryDto>();
        CreateMap<FamilyHistoryDto, FamilyHistoryUpdateDto>();
        CreateMap<FamilyHistory, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Id.ToString()));

        CreateMap<Background, BackgroundDto>();
        CreateMap<BackgroundDto, BackgroundUpdateDto>();
        CreateMap<Background, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Id.ToString()));

        CreateMap<Examination, ExaminationDto>();
        CreateMap<ExaminationDto, ExaminationUpdateDto>();
        CreateMap<Examination, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Id.ToString()));

        CreateMap<ExaminationIcd, ExaminationIcdDto>();
        CreateMap<ExaminationIcdDto, ExaminationIcd>();
        
        CreateMap<Appointment, AppointmentDto>();
        CreateMap<AppointmentWithNavigationProperties, AppointmentWithNavigationPropertiesDto>();
        CreateMap<DepartmentAppointmentCount, DepartmentAppointmentCountDto>();
        CreateMap<AppointmentDayLookupDto, AppointmentDayItemLookupDto>()
            .ForMember(dest => dest.IsSelected, opt => opt.MapFrom(src => false));

        CreateMap<Appointment, AppointmentExcelDto>()
            .ForMember(dest => dest.DoctorName,
                opt => opt.MapFrom(src => src.Doctor.FirstName + " " + src.Doctor.LastName))
            .ForMember(dest => dest.PatientName,
                opt => opt.MapFrom(src => src.Patient.FirstName + " " + src.Patient.LastName))
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.MedicalService.Name))
            .ForMember(dest => dest.PatientNumber, opt => opt.MapFrom(src => src.Patient.PatientNumber))
            .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src =>
                $"{src.StartTime:yyyy-MM-dd}, {src.StartTime:HH:mm} - {src.EndTime:HH:mm}"));


        CreateMap<Doctor, LookupDto<Guid>>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FirstName));

        CreateMap<TestCategory, TestCategoryDto>();
        CreateMap<BloodTest, BloodTestDto>();
        CreateMap<BloodTestWithNavigationProperties, BloodTestWithNavigationPropertiesDto>();

        CreateMap<BloodTestResult, BloodTestResultDto>();
        CreateMap<BloodTestResultWithNavigationProperties, BloodTestResultWithNavigationPropertiesDto>();


        CreateMap<Test, TestDto>();
        CreateMap<TestWithNavigationProperties, TestWithNavigationPropertiesDto>();

        CreateMap<Insurance, InsuranceDto>();

        CreateMap<AppointmentType, AppointmentTypeDto>();
        CreateMap<AppointmentTypeDto, AppointmentTypeUpdateDto>();
        CreateMap<AppointmentType, AppointmentTypeExcelDto>();
        CreateMap<AppointmentType, LookupDto<Guid>>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
        
        CreateMap<DoctorWorkingHour, DoctorWorkingHoursDto>();
        
        CreateMap<Insurance, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.InsuranceCompanyName.ToString()));
    }
}