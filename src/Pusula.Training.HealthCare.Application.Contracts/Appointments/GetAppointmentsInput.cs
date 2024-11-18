using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Appointments;

public class GetAppointmentsInput : PagedAndSortedResultRequestDto
{
    
    [Required]
    public Guid DoctorId { get; set; }
    
    [Required]
    public Guid MedicalServiceId { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    public GetAppointmentsInput()
    {
    }
}