using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.ProtocolTypes;

public class ProtocolTypeCreateDto
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(ProtocolTypeConsts.NameMaxLength)]
    public string Name { get; set; } = null!;
}