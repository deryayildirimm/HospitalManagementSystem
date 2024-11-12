using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Titles;

public class TitleUpdateDto
{
    [Required] 
    [StringLength(100)] 
    public Guid Id { get; set; } = default!;
    
    [Required]
    [StringLength(TitleConsts.TitleNameMaxLength)]
    public string TitleName { get; set; } = null!;
}