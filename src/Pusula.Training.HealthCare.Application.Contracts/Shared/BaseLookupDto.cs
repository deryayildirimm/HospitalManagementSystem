namespace Pusula.Training.HealthCare.Shared;

public class BaseLookupDto<TKey>
{
    public TKey Id { get; set; } = default!;

    public string DisplayName { get; set; } = default!;
}